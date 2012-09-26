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
// Original file : ViewRender.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Render content to custom view engine
	/// </summary>
	[Render("view")]
	public class ViewRender : RenderBase {
		/// <summary>
		/// </summary>
		protected IViewEngine ViewEngine {
			get { return viewEngine; }
		}


		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			base.Render(context);
			context.ContentType = "text/html";
			var viewname = context.ViewName;
			if (string.IsNullOrWhiteSpace(viewname)) {
				viewname = "/" + context.ActionName.Replace(".", "/");
			}
			if (string.IsNullOrWhiteSpace(context.MasterViewName) && !string.IsNullOrWhiteSpace(context.Get("layout"))) {
				context.MasterViewName = context.Get("layout");
			}
			ViewEngine.Process(viewname, context.MasterViewName, context.ActionResult, context);
		}

		/// <summary>
		/// 	Executes before all other calls to Action
		/// </summary>
		/// <param name="context"> </param>
		public override void SetContext(IMvcContext context) {
			if (null == viewEngine) {
				viewEngine = ResolveService<IViewEngine>();
			}
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = "text/html";
			context.Output.Write("<div class='error'><h2>error on " + context.ActionName + "</h2><textarea cols='60' rows='10'>" +
			                     error.ToString().Replace("<", "&lt;") + "</textarea></div>");
		}

		private IViewEngine viewEngine;
	}
}