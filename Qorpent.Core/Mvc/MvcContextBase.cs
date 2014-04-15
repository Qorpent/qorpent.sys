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
// PROJECT ORIGIN: Qorpent.Core/MvcContextBase.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Dsl;
using Qorpent.IoC;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Base MVC
	/// </summary>
	public abstract class MvcContextBase : ServiceBase, IMvcContext {
#if PARANOID
		static MvcContextBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif
		/// <summary>
		/// 	Regex to retrieve mvc call info from url
		/// </summary>
		protected internal const string Contextregex =
			@"^/((?<app>[^/]+)/)?(?<name>(?<root>[^/]+)/(?<leaf>[^\.]+))(\.(?<type>[^\.]+))?\.((quick)|(qweb)|(qpt))$";

		/// <summary>
		/// 	Current web request context in thread
		/// </summary>
		[IgnoreSerialize] [ThreadStatic] public static IMvcContext Current;

		/// <summary>
		/// 	Mvc factory for actions and renders
		/// </summary>
		[Inject] public IMvcFactory Factory {
			get { return _factory ?? (_factory = ResolveService<IMvcFactory>()); }
			set { _factory = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public string ApplicationName
		{
			get { return _applicationName ?? Application.ApplicationName; }
			set { _applicationName = value; }
		}

		/// <summary>
		/// Обеспечивает признак выходящего запроса - место расположения файла
		/// </summary>
		public abstract string FileDisposition { get; set; }

		/// <summary>
		/// </summary>
		public MvcCallInfo GetCallInfo() {
			var result = new MvcCallInfo
				{ActionName = ActionName, RenderName = RenderName, Url = Uri.ToString(), Parameters = Parameters};
			return result;
		}

		/// <summary>
		/// 	Set system/server defined execution context
		/// </summary>
		/// <param name="nativecontext"> </param>
		public abstract void SetNativeContext(object nativecontext);

		/// <summary>
		/// 	Output writer
		/// </summary>
		public TextWriter Output {
			get { return _output ?? (_output = new StringWriter()); }
			set { _output = value; }
		}

		/// <summary>
		/// 	Name of called action
		/// </summary>
		public virtual string ActionName {
			get {
				if (string.IsNullOrEmpty(_actionName)) {
					_actionName = MvcCallInfo.GetActionName(Uri);
				}
				return _actionName;
			}
			set { _actionName = value; }
		}

		/// <summary>
		/// 	Name of called render
		/// </summary>
		public virtual string RenderName {
			get {
				if (string.IsNullOrEmpty(_renderName)) {
					_renderName = MvcCallInfo.GetRenderName(Uri);
				}
				return _renderName;
			}
			set { _renderName = value; }
		}

		/// <summary>
		/// 	User of this mvc context
		/// </summary>
		public IPrincipal User {
			get {
				if (null == Application) {
					return LogonUser;
				}
				return Application.Principal.CurrentUser;
			}
		}

		/// <summary>
		/// 	Name of view for QView based requests
		/// </summary>
		[SerializeNotNullOnly] public string ViewName { get; set; }

		/// <summary>
		/// 	Master view name for QView based requests
		/// </summary>
		[SerializeNotNullOnly] public string MasterViewName { get; set; }

		/// <summary>
		/// 	Retrievs xml-data parameter
		/// </summary>
		[SerializeNotNullOnly] public abstract XElement XData { get; set; }

		/// <summary>
		/// 	Parameters of request
		/// </summary>
		public IDictionary<string, string> Parameters {
			get { return _parameters ?? (_parameters = RetrieveParameters()); }
		}

		/// <summary>
		/// 	Logon user - based on native HTTP context
		/// </summary>
		public abstract IPrincipal LogonUser { get; set; }

		/// <summary>
		/// 	Safe method to acess parameters in context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public abstract T Get<T>(string name, T def, bool setup = false);

		/// <summary>
		/// 	String overload for usual case
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <returns> </returns>
		public string Get(string name, string def = "", bool setup = false) {
			return Get<string>(name, def, setup);
		}

		/// <summary>
		/// 	Retrieves parameter parsed as XML
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public XElement GetXml(string name) {
			var datax = Get(name).Trim();
			if (string.IsNullOrEmpty(datax)) {
				datax = "<empty></empty>";
			}
			datax = datax.Trim();
			if(datax.Length==0)return new XElement("empty");
			//try get by usual xml
			if (datax[0]=='<') {
				return XElement.Parse(datax);
			}

			//if it's {} code - try to parse as json
			if (datax[0] == '{')
			{
				return ResolveService<ISpecialXmlParser>("json.xml.parser").ParseXml(datax);
			}
			//otherwise try parse as bxl
			return ResolveService<ISpecialXmlParser>("bxl.xml.parser").ParseXml(datax);
		}

		/// <summary>
		/// 	Converts given parameter to typed array with splitters
		/// </summary>
		/// <param name="elementtype"> </param>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public abstract Array GetArray(Type elementtype, string name, params char[] splitters);

		/// <summary>
		/// 	Converts given parameter to string array with splitters
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public abstract string[] GetArray(string name, params char[] splitters);

		/// <summary>
		/// 	Return dicitonary representation of parameters in DictionaryForm
		/// </summary>
		/// <param name="paramname"> </param>
		/// <returns> </returns>
		public abstract IDictionary<string, string> GetDict(string paramname);

		/// <summary>
		/// 	Retrievs dictionary of prefixed parameters with cropping start prefix on key
		/// </summary>
		/// <param name="prefix"> </param>
		/// <returns> </returns>
		public IEnumerable<KeyValuePair<string, string>> GetAll(string prefix) {
			return from parameter in Parameters
			       where parameter.Key.StartsWith(prefix)
			       select new KeyValuePair<string, string>(parameter.Key.Substring(prefix.Length), parameter.Value);
		}

		/// <summary>
		/// 	Safe method to setup parameter of context (flow)
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public IMvcContext Set(string name, object value) {
			Parameters[name] = value == null ? "" : value.ToString();
			return this;
		}

		/// <summary>
		/// 	Determine if request made from local system
		/// </summary>
		/// <returns> </returns>
		public abstract bool IsLocalHost();

		IApplication IMvcContext.Application {
			get { return Application; }
			set { Application = value; }
		}

		/// <summary>
		/// 	Action Descriptor, attached to context
		/// </summary>
		public ActionDescriptor ActionDescriptor {
			get { return _action ?? (_action = Factory.GetAction(this)); }
			set { _action = value; }
		}

		/// <summary>
		/// 	Render Descriptor, attached to context
		/// </summary>
		public RenderDescriptor RenderDescriptor {
			get { return _renderDescriptor ?? (_renderDescriptor = Factory.GetRender(this)); }
			set { _renderDescriptor = value; }
		}

		/// <summary>
		/// 	Uri of request
		/// </summary>
		public Uri Uri { get; set; }

		/// <summary>
		/// 	Result of authorization
		/// </summary>
		public AuthorizationResult AuthrizeResult { get; set; }

		/// <summary>
		/// 	True if this context not require Action Result
		/// </summary>
		public bool IgnoreActionResult {
			get {
				if (null == RenderDescriptor) {
					return false;
				}
				return RenderDescriptor.IgnoreActionResult;
			}
		}

		/// <summary>
		/// 	True if context is not modified (for 304 state)
		/// </summary>
		public bool NotModified { get; set; }

		/// <summary>
		/// 	Http status code
		/// </summary>
		public abstract int StatusCode { get; set; }

		/// <summary>
		/// 	Evaluated last modified state
		/// </summary>
		public abstract DateTime LastModified { get; set; }

		/// <summary>
		/// 	Evaluated etag
		/// </summary>
		public abstract string Etag { get; set; }

		/// <summary>
		/// 	Incoming if modiefied header for 304 state
		/// </summary>
		public abstract DateTime IfModifiedSince { get; set; }

		/// <summary>
		/// 	Incomiung If-None-Match header for 304 state
		/// </summary>
		public abstract string IfNoneMatch { get; set; }

		/// <summary>
		/// </summary>
		public abstract string ContentType { get; set; }

		/// <summary>
		/// 	Executes containing action
		/// </summary>
		public object Process() {
			try {
				if (null != ActionDescriptor) {
					Bind();
					return ActionResult = ActionDescriptor.Process(this);
				}
				return null;
			}
			catch (Exception ex) {
				Error = ex;
				throw;
			}
		}


		/// <summary>
		/// 	Renders error
		/// </summary>
		/// <param name="ex"> </param>
		public void RenderError(Exception ex) {
			RenderDescriptor.Render.RenderError(ex, this);
		}

		/// <summary>
		/// 	Execute free resource logic if implemented
		/// </summary>
		public virtual void Release() {
			Factory.ReleaseAction(this);
			Factory.ReleaseRender(this);
		}

		/// <summary>
		/// </summary>
		/// <param name="filename"> </param>
		public abstract void WriteOutFile(string filename);

		/// <summary>
		/// 	Executes containing render logic
		/// </summary>
		public void Render() {
			RenderDescriptor.Render.Render(this);
		}

		/// <summary>
		/// 	Error, occured in context
		/// </summary>
		public Exception Error { get; set; }

		/// <summary>
		/// 	Result of executed action
		/// </summary>
		public object ActionResult { get; set; }

		/// <summary>
		/// 	Language of request
		/// </summary>
		[SerializeNotNullOnly] public abstract string Language { get; set; }


		/// <summary>
		/// Due to remove System.Web dependency we choose to use this style of
		/// cookie support - calling site MUST use HttpCookie as object
		/// </summary>
		/// <param name="cookieObject"></param>
		public virtual void SetCookie(object cookieObject) {
			//stub
		}

		/// <summary>
		/// Due to remove System.Web dependency we choose use such ambigous
		/// method to retrieve request cookie, HttpCookie will be returned
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual object GetCookie(string name) {
			//stub
			return null;
		}

		/// <summary>
		///Признак того, что контекст вызвал Redirect
		/// </summary>
		public bool IsRedirected { get; set; }

	    /// <summary>
	    ///UserHostAddress property
	    /// </summary>
	    [SerializeNotNullOnly]
	    public virtual string UserHostAddress { get; set; }

	    /// <summary>
	    /// 	UserHostName property
	    /// </summary>
	    [SerializeNotNullOnly]
	    public virtual string UserHostName { get; set; }

	    /// <summary>
	    /// 	UserAgent property
	    /// </summary>
	    [SerializeNotNullOnly]
	    public virtual string UserAgent { get; set; }


	    /// <summary>
		/// 	Generates parameters from underlined context
		/// </summary>
		/// <returns> </returns>
		protected abstract IDictionary<string, string> RetrieveParameters();

		private ActionDescriptor _action;
		/// <summary>
		/// 
		/// </summary>
		protected string _actionName;
		private IMvcFactory _factory;
		private TextWriter _output;
		private IDictionary<string, string> _parameters;
		private RenderDescriptor _renderDescriptor;
		/// <summary>
		/// 
		/// </summary>
		protected string _renderName;

		/// <summary>
		/// 	Response redirect
		/// </summary>
		/// <param name="localurl"> </param>
		/// <returns> </returns>
		public abstract void Redirect(string localurl);

		/// <summary>
		/// Being implemented must retur HttpPostedFile, not typed because System.Web is bad dependency
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public abstract object GetFile(string filename);

		/// <summary>
		/// Выводит в исходящий поток исходный поток
		/// </summary>
		/// <param name="sourceStream"></param>
		public abstract void WriteOutStream(Stream sourceStream);

		/// <summary>
		/// Выводит в исходящий поток данные
		/// </summary>
		public abstract void WriteOutBytes(byte[] data);

		private bool _isbinded = false;
		private string _applicationName;

		/// <summary>
		/// Выполнить настройку действия на контекст
		/// </summary>
		public void Bind() {
			
			if (null != ActionDescriptor && !_isbinded)
			{
				ActionDescriptor.Bind(this);
				_isbinded = true;
			}
		}
	}
}