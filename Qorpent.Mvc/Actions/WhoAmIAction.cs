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
// Original file : WhoAmIAction.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	�������� ����������� ������ �� ������������ ������� (Logon) ������� ������
	/// </summary>
	[Action("_sys.whoami", Role = "DEFAULT", Help = "��������� �������� ��������� ������ �� ����� ������� ������")]
	public class WhoAmIAction : ActionBase {
		/// <summary>
		/// 	���������� �������� ������������
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

			var data = new Dictionary<string, string>();

			
			
			foreach (var h in ((MvcContext)Context).NativeASPContext.Request.Headers.AllKeys) {
				data["header:"+h] = ((MvcContext) Context).NativeASPContext.Request.Headers[h];
			}
			foreach (var h in ((MvcContext)Context).NativeASPContext.Request.Cookies.AllKeys)
			{
				data["cookie:"+h] = ((MvcContext)Context).NativeASPContext.Request.Cookies[h].Value;
			}

			if (imp == logon) {
				return new {logonname, logonadmin, logondeveloper, logondatamaster,data

				
				};
			}

			var impadmin = Application.Roles.IsInRole(imp, "ADMIN");
			var impdeveloper = Application.Roles.IsInRole(imp, "DEVELOPER");
			var impdatamaster = Application.Roles.IsInRole(imp, "DATAMASTER");

			return
				new {impersonation, impadmin, impdeveloper, impdatamaster, logonname, logonadmin, logondeveloper, logondatamaster
				};
		}
	}
}