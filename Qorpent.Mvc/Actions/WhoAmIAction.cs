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
			var logonadmin = Application.Roles.IsInRole(logon.Identity, "ADMIN");

            var logondeveloper = Application.Roles.IsInRole(logon.Identity, "DEVELOPER");
            var logondatamaster = Application.Roles.IsInRole(logon.Identity, "DATAMASTER");
            var logondesigner = Application.Roles.IsInRole(logon.Identity, "DESIGNER");
            var logontester = Application.Roles.IsInRole(logon.Identity, "TESTER");
            var logonbudget = Application.Roles.IsInRole(logon.Identity, "BUDGET");
            var logondocwriter = Application.Roles.IsInRole(logon.Identity, "DOCWRITER");


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
						logondocwriter
					};
			}

            var impadmin = Application.Roles.IsInRole(imp.Identity, "ADMIN");
            var impdeveloper = Application.Roles.IsInRole(imp.Identity, "DEVELOPER");
            var impdatamaster = Application.Roles.IsInRole(imp.Identity, "DATAMASTER");
            var impdatatester = Application.Roles.IsInRole(imp.Identity, "TESTER");
            var impbudget = Application.Roles.IsInRole(imp.Identity, "BUDGET");
            var impdocwriter = Application.Roles.IsInRole(imp.Identity, "DOCWRITER");

			return
				new
					{
						impersonation,
						impadmin,
						impdeveloper,
						impdatamaster,
						impdatatester,
						impbudget,
						impdocwriter,
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