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
// PROJECT ORIGIN: Qorpent.Mvc/WhoAmIAction.cs
#endregion
using System.Collections.Generic;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Вызывает регистрацию записи об имперсонации текущей (Logon) учетной записи
	/// </summary>
	[Action("_sys.whoami", Role = "DEFAULT", Help = "Позволяет получить первичные данные по своей учетной записи")]
	public class WhoAmIAction : ActionBase {
		/// <summary>
		/// 	Собственно вызывает имперсонацию
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var logon = Context.LogonUser;
			var imp = Application.Impersonation.GetImpersonation(logon);


			var logonname = Context.LogonUser.Identity.Name;
			var impersonation = Application.Impersonation.GetImpersonation(Context.LogonUser).Identity.Name;
			var logonadmin = Application.Roles.IsInRole(logon, "ADMIN");

			var logondeveloper = Application.Roles.IsInRole(logon, "DEVELOPER");
			var logondatamaster = Application.Roles.IsInRole(logon, "DATAMASTER");
			var logondesigner = Application.Roles.IsInRole(logon, "DESIGNER");
			var logontester = Application.Roles.IsInRole(logon, "TESTER");
			var logonbudget = Application.Roles.IsInRole(logon, "BUDGET");

			//var data = new Dictionary<string, string>();


			//foreach (var h in ((MvcContext) Context).NativeAspContext.Request.Headers.AllKeys) {
			//	data["header:" + h] = ((MvcContext) Context).NativeAspContext.Request.Headers[h];
			//}
			//foreach (var h in ((MvcContext) Context).NativeAspContext.Request.Cookies.AllKeys) {
			//	var httpCookie = ((MvcContext) Context).NativeAspContext.Request.Cookies[h];
			//	if (httpCookie != null) {
			//		data["cookie:" + h] = httpCookie.Value;
			//	}
			//}

			if (imp == logon) {
				return new
					{
						logonname,
						logonadmin,
						logondeveloper,
						logondatamaster,
						logondesigner,
						logontester,
						logonbudget,
					};
			}

			var impadmin = Application.Roles.IsInRole(imp, "ADMIN");
			var impdeveloper = Application.Roles.IsInRole(imp, "DEVELOPER");
			var impdatamaster = Application.Roles.IsInRole(imp, "DATAMASTER");
			var impdatatester = Application.Roles.IsInRole(imp, "TESTER");
			var impbudget = Application.Roles.IsInRole(imp, "BUDGET");

			return
				new
					{
						impersonation,
						impadmin,
						impdeveloper,
						impdatamaster,
						impdatatester,
						impbudget,
						logonname,
						logonadmin,
						logondeveloper,
						logondatamaster,
						logontester,
						logonbudget,
						
					};
		}
	}
}