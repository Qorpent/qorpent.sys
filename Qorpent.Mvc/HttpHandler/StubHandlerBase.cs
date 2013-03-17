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
// PROJECT ORIGIN: Qorpent.Mvc/StubHandlerBase.cs
#endregion
using System.Web;
using Qorpent.Applications;

namespace Qorpent.Mvc.HttpHandler {
	/// <summary>
	/// 	Handler, used for special cases
	/// </summary>
	public class StubHandlerBase {
		/// <summary>
		/// </summary>
		public IApplication Application { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="statestring"> </param>
		/// <param name="statecode"> </param>
		/// <param name="enstring"> </param>
		/// <param name="rustring"> </param>
		protected static void Writeout(HttpContext context, string statestring, int statecode, string enstring,
		                               string rustring) {
			//context.Response.Status = statestring;
			context.Response.StatusCode = statecode;

			//context.Response.StatusDescription = enstring + Environment.NewLine + rustring;

			if (context.Request.Url.ToString().Contains(".xml.qweb")) {
				context.Response.ContentType = "text/xml";
				context.Response.Write("<info><en>" + enstring + "</en><ru>" + rustring + "</ru></info>");
			}
			else if (context.Request.Url.ToString().Contains(".json.qweb")) {
				context.Response.ContentType = "application/json";
				context.Response.Write("{'info':{'startup':'" + enstring + "', ru: '" + rustring + "' }}");
			}
			else if (context.Request.Url.ToString().Contains(".js.qweb")) {
				context.Response.ContentType = "text/javascript";
				context.Response.Write("{'info':{'startup':'" + enstring + "', ru: '" + rustring + "' }}");
			}
			else if (context.Request.Url.ToString().Contains(".bxl.qweb")) {
				context.Response.ContentType = "text/handler";
				context.Response.Write("info\r\n\tstartup:'''" + enstring + "'''\r\n\tru:'''" + rustring + "'''");
			}
			else if (context.Request.Url.ToString().Contains("view.qweb")) {
				context.Response.ContentType = "text/html";
				context.Response.Write("<h1><p>" + enstring + "<p><p>" + rustring + "</p></h1>");
			}
			else {
				context.Response.ContentType = "text/xml";
				context.Response.Write("<info><en>" + enstring + "</en><ru>" + rustring + "</ru></info>");
			}
			context.Response.Flush();
			context.Response.Close();
		}
	}
}