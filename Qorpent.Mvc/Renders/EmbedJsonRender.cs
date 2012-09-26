#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : EmbedJsonRender.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	this render usefull for cached application api - wraps JS result to call of storing
	/// 	js object into window.qweb.embedStorage[actionname]
	/// </summary>
	[Render("embedjson")]
	public class EmbedJsonRender : JsonRender {
		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			context.ContentType = context.Get("ajax", false) ? "application/json" : "text/javascript";

			context.Output.Write(
				@"
			if(!window.qweb)window.qweb={};
			if(!window.qweb.embedStorage)window.qweb.embedStorage={};
			window.qweb.embedStorage.");
			context.Output.Write(context.ActionName.Replace(".", "__"));
			context.Output.Write("=");
			if (null == context.ActionResult) {
				context.Output.Write("null");
			}
			else {
				GetSerializer().Serialize("result", context.ActionResult, context.Output);
			}
			context.Output.Write(";");
		}
	}
}