#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Mvc/MvcContext.cs
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Dsl;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Default implementation of normal MvcContext - will be used as default MvcContext service in applications
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class MvcContext : MvcContextBase {
		/// <summary>
		/// </summary>
		/// <param name="httpContextWrapper"> </param>
		public MvcContext(System.Web.HttpContextWrapper httpContextWrapper) {
			InitializeContext(httpContextWrapper);
			
		}

		private void InitializeContext(System.Web.HttpContextWrapper httpContextWrapper) {
			SetNativeContext(httpContextWrapper);
		}


		/// <summary>
		/// </summary>
		public MvcContext() {}

		/// <summary>
		/// </summary>
		/// <param name="url"> </param>
		public MvcContext(string url) {
			Uri = new Uri(url);
		}

		/// <summary>
		/// 	Cookie отклика
		/// </summary>
		public HttpCookieCollection ResponseCookies {
			get {
				if (null != NativeAspContext) {
					return NativeAspContext.Response.Cookies;
				}
				return null;
			}
		}

		/// <summary>
		/// 	Cookie отклика
		/// </summary>
		public HttpCookieCollection RequestCookies {
			get {
				if (null != NativeAspContext) {
					return NativeAspContext.Request.Cookies;
				}
				return null;
			}
		}

		/// <summary>
		/// real implementation that works with response cookies
		/// </summary>
		/// <param name="cookieObject"></param>
		public override void SetCookie(object cookieObject)
		{
			ResponseCookies.Set((HttpCookie)cookieObject);
		}

		/// <summary>
		/// real implementation that works with request cookies
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetCookie(string name) {
			return RequestCookies.Get(name);
		}

		/// <summary>
		/// Обеспечивает признак выходящего запроса - место расположения файла
		/// </summary>
		public override string FileDisposition {
			get {
				if(null!=NativeAspContext) {
					//Берем хидер с удалением из него обвязки
					return NativeAspContext.Response.Headers["Content-Disposition"].Replace("filename=\"","").Replace("\"","").Replace("'","\"");

					 
				}
				return _fileDisposition;
			}
			set {
				if(null!=NativeAspContext) {
					NativeAspContext.Response.Headers["Content-Disposition"] = "filename=\"" + value.Replace("\"", "'")+"\"";
					return;
				}
				_fileDisposition = value;
			}
		}
		/// <summary>
		/// Выводит в исходящий поток исходный поток
		/// </summary>
		/// <param name="sourceStream"></param>
		public override void WriteOutStream(Stream sourceStream) {
			if(null==NativeAspContext) {
				throw new Exception("cannot write out stream without native context");
			}
			sourceStream.CopyTo(NativeAspContext.Response.OutputStream);
		}

		/// <summary>
		/// 	IIS-based HTTP context (mostly used)
		/// </summary>
		[IgnoreSerialize] public HttpContextBase NativeAspContext { get; protected set; }

		/// <summary>
		/// 	Retrievs xml-data parameter
		/// </summary>
		public override XElement XData {
			get {
				if (null == _xdata && !_xdatachecked) {
					if (Parameters.ContainsKey("_xdata")) {
						var x = Get("_xdata");
						if (!string.IsNullOrEmpty(x)) {
							_xdata = XElement.Parse(x);
						}
					}
					else if (Parameters.ContainsKey("_jdata")) {
						var j = Get("_jdata");
						if (!string.IsNullOrEmpty(j)) {
							_xdata = ResolveService<ISpecialXmlParser>("json.xml.parser").Parse(j);
						}
					}
					else if (Parameters.ContainsKey("_bxldata")) {
						var bxl = Get("_bxldata");
						if (bxl.IsNotEmpty()) {
							_xdata = ResolveService<IBxlParser>().Parse(bxl, "bxldata",
							                                            BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
						}
					}

					_xdatachecked = true;
				}

				return _xdata;
			}
			set {
				_xdata = value;
				_xdatachecked = true;
			}
		}

		/// <summary>
		/// 	Logon user - based on native HTTP context
		/// </summary>
		public override IPrincipal LogonUser {
			get {
				if (null == _logonuser) {
					_logonuser =
						null != NativeAspContext
							? NativeAspContext.User
							: new GenericPrincipal(new GenericIdentity("local\\guest"), new[] {"DEFAULT"});
					//SETUP USER FROM APACHE BASIC AUTHORIZATION HEADER
					if ((string.IsNullOrEmpty(_logonuser.Identity.Name) || _logonuser.Identity.Name == "local\\guest") &&
					    NativeAspContext != null && NativeAspContext.Request.Headers.AllKeys.Any(x => x == "Authorization")) {
						var auth = NativeAspContext.Request.Headers["Authorization"];
						if (auth.StartsWith("Basic")) {
							var namepass = auth.Split(' ')[1].Trim();

							var name = Encoding.UTF8.GetString(Convert.FromBase64String(namepass));
							name = name.Split(':')[0].Trim();
							_logonuser = new GenericPrincipal(new GenericIdentity("local\\" + name), new[] {"DEFAULT"});
						}
					}
				}
				return _logonuser;
			}
			set { _logonuser = value; }
		}

		/// <summary>
		/// 	True if HTTP headers are supported
		/// </summary>
		public bool SupportHeaders { get; set; }

		/// <summary>
		/// 	Http status code
		/// </summary>
		public override int StatusCode {
			get {
				if (null != NativeAspContext) {
					return NativeAspContext.Response.StatusCode;
				}
				return _statusCode;
			}
			set {
				if (null != NativeAspContext) {
					NativeAspContext.Response.StatusCode = value;
				}
				_statusCode = value;
			}
		}

		/// <summary>
		/// 	Evaluated last modified state
		/// </summary>
		[SerializeNotNullOnly] public override DateTime LastModified {
			get { return _lastModified; }
			set {
				_lastModified = value;
				if (SupportHeaders) {
					NativeAspContext.Response.Headers["Last-Modified"] = value.ToUniversalTime().ToString("R",
					                                                                                      CultureInfo.InvariantCulture);
				}
			}
		}


		/// <summary>
		/// 	Evaluated etag
		/// </summary>
		[SerializeNotNullOnly] public override string Etag {
			get { return _etag; }
			set {
				_etag = value ?? "";
				if (SupportHeaders) {
					NativeAspContext.Response.Headers["Etag"] = value ?? "";
				}
			}
		}

		/// <summary>
		/// 	Incoming if modiefied header for 304 state
		/// </summary>
		[SerializeNotNullOnly] public override DateTime IfModifiedSince {
			get {
				if (_ifModifiedSince == DateTime.MinValue) {
					_ifModifiedSince = new DateTime(1900, 1, 1);
					if (NativeAspContext != null) {
						var header = NativeAspContext.Request.Headers["If-Modified-Since"];
						if (header.IsNotEmpty()) {
							_ifModifiedSince =
								DateTime.ParseExact(header, "R", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToLocalTime();
							_ifModifiedSince = new DateTime(IfModifiedSince.Year, IfModifiedSince.Month, IfModifiedSince.Day,
							                                IfModifiedSince.Hour, IfModifiedSince.Minute, IfModifiedSince.Second);
						}
					}
				}
				return _ifModifiedSince;
			}
			set { _ifModifiedSince = value; }
		}

		/// <summary>
		/// 	Incomiung If-None-Match header for 304 state
		/// </summary>
		[SerializeNotNullOnly] public override string IfNoneMatch {
			get {
				if (_ifNoneMatch == null) {
					_ifNoneMatch = "";
					if (SupportHeaders) {
						var header = NativeAspContext.Request.Headers["If-None-Match"];
						if (header.IsNotEmpty()) {
							_ifNoneMatch = header;
						}
					}
				}
				return _ifNoneMatch;
			}
			set { _ifNoneMatch = value; }
		}

		/// <summary>
		/// 	Language of request
		/// </summary>
		[SerializeNotNullOnly] public override string Language {
			get {
				if (null == _language) {
					if (null != NativeAspContext && SupportHeaders) {
						_language = NativeAspContext.Request.Headers["Accept-Language"];
					}
				}
				return _language;
			}
			set { _language = value; }
		}
        /// <summary>
        /// 	UserHostAddress property
        /// </summary>
        [SerializeNotNullOnly]
        public override string UserHostAddress
        {
            get
            {
                if (null == _userhostaddress)
                {
                    if (null != NativeAspContext)
                    {
                        _userhostaddress = NativeAspContext.Request.UserHostAddress;
                    }
                }
                return _userhostaddress;
            }
            set { _userhostaddress = value; }
        }

        /// <summary>
        /// 	UserHostName property
        /// </summary>
        [SerializeNotNullOnly]
        public override string UserHostName
        {
            get
            {
                if (null == _userhostname)
                {
                    if (null != NativeAspContext)
                    {
                        _userhostname = NativeAspContext.Request.UserHostName;
                    }
                }
                return _userhostname;
            }
            set { _userhostname = value; }
        }

        /// <summary>
        /// 	UserAgent property
        /// </summary>
        [SerializeNotNullOnly]
        public override string UserAgent
        {
            get
            {
                if (null == _useragent)
                {
                    if (null != NativeAspContext)
                    {
                        _useragent = NativeAspContext.Request.UserAgent;
                    }
                }
                return _useragent;
            }
            set { _useragent = value; }
        }

		/// <summary>
		/// </summary>
		public override string ContentType {
			get {
				if (null != NativeAspContext) {
					return NativeAspContext.Response.ContentType;
				}
				return _contentType;
			}
			set {
				_contentType = value;
				if (null != NativeAspContext) {
					NativeAspContext.Response.ContentType = value;
				}
			}
		}

		/// <summary>
		/// 	Response redirect
		/// </summary>
		/// <param name="localurl"> </param>
		/// <returns> </returns>
		public override void Redirect(string localurl) {
			var prefix = "/" + Application.ApplicationName + "/";
			if (!localurl.StartsWith(prefix)) {
				localurl = prefix + localurl;
			}
			NativeAspContext.Response.Redirect(localurl, false);
			IsRedirected = true;
		}

		/// <summary>
		/// Being implemented must retur HttpPostedFile, not typed because System.Web is bad dependency
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public override object GetFile(string filename) {
			if(null==NativeAspContext) return null;
			if(NativeAspContext.Request.Files.AllKeys.Any(_=>_==filename)) {
				return NativeAspContext.Request.Files[filename];
			}
			return null;
		}

		/// <summary>
		/// 	Write out a file
		/// </summary>
		/// <param name="filename"> </param>
		/// <exception cref="NotSupportedException"></exception>
		public override void WriteOutFile(string filename) {
			if (null == NativeAspContext) {
				throw new NotSupportedException("only for attached to native Context");
			}
			NativeAspContext.Response.WriteFile(filename);
		}

		/// <summary>
		/// 	Set system/server defined execution context
		/// </summary>
		/// <param name="nativecontext"> </param>
		public override void SetNativeContext(object nativecontext) {
			NativeAspContext = (System.Web.HttpContextWrapper) nativecontext;
			SupportHeaders = true;
			try {
				//try call headers - here we see does underlined host support headers
			}
			catch (PlatformNotSupportedException) {
				SupportHeaders = false;
			}

			Uri = NativeAspContext.Request.Url;
			Language = null != NativeAspContext.Request.UserLanguages ? NativeAspContext.Request.UserLanguages.FirstOrDefault() : CultureInfo.CurrentCulture.Name;
			Output = NativeAspContext.Response.Output;
		}

		/// <summary>
		/// 	Generates parameters from underlined context
		/// </summary>
		/// <returns> </returns>
		protected override IDictionary<string, string> RetrieveParameters() {
			var result = new Dictionary<string, string>();
			if (null != NativeAspContext) {
				foreach (var v in NativeAspContext.Request.Form.AllKeys) {
					if (null != v) {
						result[v.ToLower()] = NativeAspContext.Request.Form[v];
					}
				}
				foreach (var v in NativeAspContext.Request.QueryString.AllKeys) {
					if (null != v) {
						result[v.ToLower()] = NativeAspContext.Request.QueryString[v];
					}
				}
			}
			else {
				if (null != Uri) {
					var srcctring = Uri.Query;
					var parameters = Regex.Matches(srcctring, @"([^&=\?]+)=([^&=\?]+)", RegexOptions.Compiled);
					foreach (Match parameter in parameters) {
						var name = Uri.UnescapeDataString(parameter.Groups[1].Value);
						var value = Uri.UnescapeDataString(parameter.Groups[2].Value);
						result[name] = value;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 	Safe method to acess parameters in context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
// ReSharper disable OptionalParameterHierarchyMismatch
		public override T Get<T>(string name, T def = default(T), bool setup = false) {
// ReSharper restore OptionalParameterHierarchyMismatch
			if (Parameters.ContainsKey(name) || null == XData) {
				return Parameters.GetValue(name, def, initialize: setup);
			}
			var a = XData.Attribute(name);
			if (null != a) {
				if (typeof (XAttribute).IsAssignableFrom(typeof (T))) {
					return (T) (object) a;
				}
				return a.Value.To<T>();
			}
			var e = XData.Element(name);
			if (null != e) {
				if (typeof (XElement).IsAssignableFrom(typeof (T))) {
					return (T) (object) e;
				}
				return e.Value.To<T>();
			}
			return def;
		}

		/// <summary>
		/// 	Converts given parameter to typed array with splitters
		/// </summary>
		/// <param name="elementtype"> </param>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public override Array GetArray(Type elementtype, string name, params char[] splitters) {
			var strarray = GetArray(name, splitters).Select(x => x.ToTargetType(elementtype)).ToArray();
			var result = Array.CreateInstance(elementtype, strarray.Length);
			Array.Copy(strarray, result, strarray.Length);
			return result;
		}

		/// <summary>
		/// 	Converts given parameter to string array with splitters
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public override string[] GetArray(string name, params char[] splitters) {
			splitters = splitters.Length == 0 ? new[] {','} : splitters;
			if (Parameters.ContainsKey(name)) {
				var arraysrc = Parameters[name];
				return arraysrc.SmartSplit(false, true, splitters).ToArray();
			}
			if (null != XData) {
				var a = XData.Attribute(name);
				if (null != a) {
					return a.Value.SmartSplit(false, true, splitters).ToArray();
				}
				var elements = XData.Elements(name).ToArray();
				if (elements.Count() > 1) {
					return elements.Select(x => x.Value).ToArray();
				}
				if (elements.Count() == 1) {
					var e = elements.First(x=>null!=x);
					if (e.Elements().Any()) {
						return e.Elements().Select(x => x.Value).ToArray();
					}
					return new[] {e.Value};
				}
			}
			return new string[] {};
		}


		/// <summary>
		/// 	Return dicitonary representation of parameters in DictionaryForm
		/// </summary>
		/// <param name="paramname"> </param>
		/// <returns> </returns>
		public override IDictionary<string, string> GetDict(string paramname) {
			var prefix = paramname + ".";
			var result = new Dictionary<string, string>();
			if (null != Parameters.Keys.FirstOrDefault(x => x.StartsWith(prefix))) {
				// return by query string logic
				foreach (var parameter in Parameters) {
					if (parameter.Key.StartsWith(prefix)) {
						var key = parameter.Key.Split('.')[1];
						var val = parameter.Value;
						result[key] = val;
					}
				}
			}
			else if (null != XData) {
				if (null != XData.Attributes().FirstOrDefault(x => x.Name.LocalName.StartsWith(prefix))) {
					//attribute-named logic
					foreach (var attr in XData.Attributes()) {
						if (attr.Name.LocalName.StartsWith(prefix)) {
							var key = attr.Name.LocalName.Split('.')[1];
							var val = attr.Value;
							result[key] = val;
						}
					}
				}
				else if (null != XData.Elements().FirstOrDefault(x => x.Name.LocalName == paramname && null != x.Attribute("id"))) {
					// plain element style
					foreach (var element in XData.Elements()) {
						if (element.Name.LocalName == paramname) {
							var key = element.Attr("id");
							string val = null != element.Attribute("value") ? element.Attr("value") : element.Value;
							result[key] = val;
						}
					}
				}
				else if (null != XData.Elements().FirstOrDefault(x => x.Name == paramname && x.Elements().Any())) {
					//elements as name or src
					var dictsrs = XData.Element(paramname);
					if (dictsrs != null) {
						foreach (var element in dictsrs.Elements()) {
							var key = element.Name.LocalName;
							if (null != element.Attribute("id")) {
								var xAttribute = element.Attribute("id");
								if (xAttribute != null) {
									key = xAttribute.Value;
								}
							}
							var val = element.Value;
							if (null != element.Attribute("value")) {
								var xAttribute = element.Attribute("value");
								if (xAttribute != null) {
									val = xAttribute.Value;
								}
							}

							result[key] = val;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 	Determine if request made from local system
		/// </summary>
		/// <returns> </returns>
		public override bool IsLocalHost() {
			try {
				if (NativeAspContext == null && Uri.Host == "localhost") {
					return true; //called in embeded mode - no role checking needed by default if specially local url used
				}
				if (NativeAspContext != null
				    &&
				    (
					    NativeAspContext.Request.UserHostAddress == "127.0.0.1" ||
					    NativeAspContext.Request.UserHostAddress == "::1"
				    )
					) {
					return true;
				}
				return false;
			}
			catch {
				return false;
			}
		}

		private string _contentType;
		private string _etag;
		private DateTime _ifModifiedSince;
		private string _ifNoneMatch;
		private string _language;
		private DateTime _lastModified;
		private IPrincipal _logonuser;
		private int _statusCode;
		private XElement _xdata;
		private bool _xdatachecked;
		private string _fileDisposition;
	    private string _userhostaddress;
	    private string _userhostname;
	    private string _useragent;
	}
}