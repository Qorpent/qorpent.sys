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
// PROJECT ORIGIN: Qorpent.Core/IMvcContext.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Context of current request (in web terminology)
	/// </summary>
	public interface IMvcContext {
		/// <summary>
		/// 	Output writer
		/// </summary>
		TextWriter Output { get; set; }

	   

		/// <summary>
		/// 	Name of called action
		/// </summary>
		string ActionName { get; set; }

		/// <summary>
		/// 	Name of called render
		/// </summary>
		string RenderName { get; set; }

		/// <summary>
		/// 	User of this mvc context
		/// </summary>
		IPrincipal User { get; }

		/// <summary>
		/// 	Name of view for QView based requests
		/// </summary>
		[SerializeNotNullOnly] string ViewName { get; set; }

		/// <summary>
		/// 	Master view name for QView based requests
		/// </summary>
		[SerializeNotNullOnly] string MasterViewName { get; set; }

		/// <summary>
		/// 	Retrievs xml-data parameter
		/// </summary>
		[SerializeNotNullOnly] XElement XData { get; set; }

		/// <summary>
		/// 	Parameters of request
		/// </summary>
		IDictionary<string, string> Parameters { get; }

		/// <summary>
		/// 	Logon user - based on native HTTP context
		/// </summary>
		IPrincipal LogonUser { get; set; }


		/// <summary>
		/// 	Containing application
		/// </summary>
		IApplication Application { get; set; }


		/// <summary>
		/// 	Action Descriptor, attached to context
		/// </summary>
		ActionDescriptor ActionDescriptor { get; set; }


		/// <summary>
		/// 	Render Descriptor, attached to context
		/// </summary>
		RenderDescriptor RenderDescriptor { get; set; }

		/// <summary>
		/// 	Uri of request
		/// </summary>
		Uri Uri { get; set; }

		/// <summary>
		/// 	Result of authorization
		/// </summary>
		AuthorizationResult AuthrizeResult { get; set; }

		/// <summary>
		/// 	True if this context not require Action Result
		/// </summary>
		bool IgnoreActionResult { get; }

		/// <summary>
		/// 	True if context is not modified (for 304 state)
		/// </summary>
		bool NotModified { get; set; }

		/// <summary>
		/// 	Result of executed action
		/// </summary>
		object ActionResult { get; set; }

		/// <summary>
		/// 	Error, occured in context
		/// </summary>
		Exception Error { get; set; }

		/// <summary>
		/// 	Http status code
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// 	Evaluated last modified state
		/// </summary>
		DateTime LastModified { get; set; }

		/// <summary>
		/// 	Evaluated etag
		/// </summary>
		string Etag { get; set; }

		/// <summary>
		/// 	Incoming if modiefied header for 304 state
		/// </summary>
		DateTime IfModifiedSince { get; set; }

		/// <summary>
		/// 	Incomiung If-None-Match header for 304 state
		/// </summary>
		string IfNoneMatch { get; set; }

		/// <summary>
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// 	Language of request
		/// </summary>
		[SerializeNotNullOnly] string Language { get; set; }


		/// <summary>
		/// Due to remove System.Web dependency we choose to use this style of
		/// cookie support - calling site MUST use HttpCookie as object
		/// </summary>
		/// <param name="cookieObject"></param>
		void SetCookie(object cookieObject);
		/// <summary>
		/// Due to remove System.Web dependency we choose use such ambigous
		/// method to retrieve request cookie, HttpCookie will be returned
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetCookie(string name);

		/// <summary>
		/// 	Признак того, что контекст вызвал Redirect
		/// </summary>
		bool IsRedirected { get; set; }
		/// <summary>
		/// Обеспечивает признак выходящего запроса - место расположения файла
		/// </summary>
		string FileDisposition { get; set; }

	    /// <summary>
	    /// 	UserHostAddress property
	    /// </summary>
	    [SerializeNotNullOnly]
	    string UserHostAddress { get;  }

	    /// <summary>
	    /// 	UserHostName property
	    /// </summary>
	    [SerializeNotNullOnly]
	    string UserHostName { get;  }

	    /// <summary>
	    /// 	UserAgent property
	    /// </summary>
	    [SerializeNotNullOnly]
	    string UserAgent { get;  }

	    /// <summary>
		/// 	Extract call only information from context (for serialization propose)
		/// </summary>
		MvcCallInfo GetCallInfo();

		/// <summary>
		/// 	Set system/server defined execution context
		/// </summary>
		/// <param name="nativecontext"> </param>
		void SetNativeContext(object nativecontext);

		/// <summary>
		/// 	Safe method to acess parameters in context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		T Get<T>(string name, T def, bool setup = false);

		/// <summary>
		/// 	String overload for usual case
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <returns> </returns>
		string Get(string name, string def = "", bool setup = false);

		/// <summary>
		/// 	Retrieves parameter parsed as XML
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		XElement GetXml(string name);

		/// <summary>
		/// 	Converts given parameter to typed array with splitters
		/// </summary>
		/// <param name="elementtype"> </param>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		Array GetArray(Type elementtype, string name, params char[] splitters);

		/// <summary>
		/// 	Converts given parameter to string array with splitters
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		string[] GetArray(string name, params char[] splitters);


		/// <summary>
		/// 	Return dicitonary representation of parameters in DictionaryForm
		/// </summary>
		/// <param name="paramname"> </param>
		/// <returns> </returns>
		IDictionary<string, string> GetDict(string paramname);

		/// <summary>
		/// 	Retrievs dictionary of prefixed parameters with cropping start prefix on key
		/// </summary>
		/// <param name="prefix"> </param>
		/// <returns> </returns>
		IEnumerable<KeyValuePair<string, string>> GetAll(string prefix);

		/// <summary>
		/// 	Safe method to setup parameter of context (flow)
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		IMvcContext Set(string name, object value);

		/// <summary>
		/// 	Determine if request made from local system
		/// </summary>
		/// <returns> </returns>
		bool IsLocalHost();

		/// <summary>
		/// 	Executes containing action
		/// </summary>
		object Process();

		/// <summary>
		/// 	Executes containing render logic
		/// </summary>
		void Render();

		/// <summary>
		/// 	Renders error
		/// </summary>
		/// <param name="ex"> </param>
		void RenderError(Exception ex);

		/// <summary>
		/// 	Execute free resource logic if implemented
		/// </summary>
		void Release();

		/// <summary>
		/// 	Write out a file
		/// </summary>
		/// <param name="filename"> </param>
		/// <exception cref="NotSupportedException"></exception>
		void WriteOutFile(string filename);

		/// <summary>
		/// 	Response redirect
		/// </summary>
		/// <param name="localurl"> </param>
		/// <returns> </returns>
		void Redirect(string localurl);
		/// <summary>
		/// Being implemented must retur HttpPostedFile, not typed because System.Web is bad dependency
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		object GetFile(string filename);

		/// <summary>
		/// Выводит в исходящий поток исходный поток
		/// </summary>
		/// <param name="sourceStream"></param>
		void WriteOutStream(Stream sourceStream);
		/// <summary>
		/// Выводит в исходящий поток данные
		/// </summary>
		void WriteOutBytes(byte[] data);
		/// <summary>
		/// Выполнить настройку действия на контекст
		/// </summary>
		void Bind();
	}
}