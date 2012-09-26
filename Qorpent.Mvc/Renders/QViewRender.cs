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
// Original file : QViewRender.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Diagnostics;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Render output with QView
	/// </summary>
	[Render("qview")]
	public class QViewRender : RenderBase {
		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			context.ContentType = "text/html";
			var viewname = GetViewName(context);
			if (string.IsNullOrWhiteSpace(viewname)) {
				viewname = ("/" + context.ActionName.Replace(".", "/")).Replace("//", "/");
			}
			context.MasterViewName = GetLayout(context);
			IQViewContext ctx = new QViewContext();
			ctx.Name = viewname;
			ctx.Output = context.Output;
			ctx.Master = context.MasterViewName;
			ctx.ViewData = GetViewData(context);
			ctx.Context = context;
			ctx.OutputErrors = context.Get("outputerrors", true);
			ctx.Factory = Descriptor.Factory;
			ctx = ctx.GetNormalizedContext();
			var view = Descriptor.Factory.GetView(ctx.Name);
			var sw = Stopwatch.StartNew();
			try {
				view.Process(ctx);
			}
			finally {
				Descriptor.Factory.ReleaseView(ctx.Name, view);
			}

			sw.Stop();
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

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected virtual object GetViewData(IMvcContext context) {
			return context.ActionResult;
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected virtual string GetLayout(IMvcContext context) {
			var layout = context.MasterViewName;
			if (string.IsNullOrWhiteSpace(context.MasterViewName) && !string.IsNullOrWhiteSpace(context.Get("layout"))) {
				layout = context.Get("layout");
			}
			return layout;
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected virtual string GetViewName(IMvcContext context) {
			return context.ViewName;
		}
	}
}