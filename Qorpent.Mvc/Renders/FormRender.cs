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
// PROJECT ORIGIN: Qorpent.Mvc/FormRender.cs
#endregion
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Render user HTML form to make call to MVC action
	/// </summary>
	[Render("form")]
	public class FormRender : QViewRender {
		/// <summary>
		/// 	False if it's self-made render which not require call to action to get user response
		/// </summary>
		public override bool NeedResult {
			get { return false; }
		}


		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected override object GetViewData(IMvcContext context) {
			return context.ActionDescriptor;
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected override string GetViewName(IMvcContext context) {
			return "_sys/actionform";
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		protected override string GetLayout(IMvcContext context) {
			var result = base.GetLayout(context);
			if (result.IsNotEmpty()) {
				return result;
			}
			if (context.Get("ajax", false)) {
				return "";
			}
			return "default";
		}
	}
}