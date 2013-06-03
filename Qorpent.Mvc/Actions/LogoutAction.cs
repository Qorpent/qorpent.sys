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
// PROJECT ORIGIN: Qorpent.Mvc/LogoutAction.cs
#endregion
using System;
using System.Web;
using System.Web.Security;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Simple action for form authentication
	/// </summary>
	[Action("_sys.logout", Help = "EN: used to logout if forms used", Role = "DEFAULT")]
	public class LogoutAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
#if PARANOID
			var cookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, false);
			if(Paranoid.Provider.IsSpecialUser(User)) {
				Paranoid.Provider.RemoveLogin(User.Identity.Name,cookie.Value);
			}
#endif
			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "") {Expires = DateTime.Now.AddYears(-1)};
			if (cookie.Domain == null)
			{
				cookie.Domain = ((MvcContext)Context).NativeAspContext.Request.Url.Host;
			}
			var domainparts = cookie.Domain.Split('.');
			if (domainparts.Length == 3)
			{
				cookie.Domain = domainparts[1] + "." + domainparts[2];
			}
			Context.SetCookie(cookie);
			//FormsAuthentication.SignOut();
			Context.Redirect("_sys/login.qview.qweb");
			return true;
		}
	}
}