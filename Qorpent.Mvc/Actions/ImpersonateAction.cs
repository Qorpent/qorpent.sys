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
// PROJECT ORIGIN: Qorpent.Mvc/ImpersonateAction.cs
#endregion
using System.Security.Principal;
using Qorpent.Mvc.Binding;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	�������� ����������� ������ �� ������������ ������� (Logon) ������� ������  - ��������� ���� DEFAULT - �������� ��������
	/// 	������ �� Logon
	/// </summary>
	[Action("_sys.impersonate", Role = "DEFAULT", Help = "��������� ����������� ���� �� ����� ������� ������������")]
	public class ImpersonateAction : ActionBase {
		/// <summary>
		/// 	������� ������� ������
		/// </summary>
		[Bind] public string Target { get; set; }


		/// <summary>
		/// 	4 part of execution - all internal context is ready - authorize it with custom logic
		/// </summary>
		protected override void Authorize() {
			if (Target.IsEmpty()) {
				return;
			}
            if (Application.Roles.IsInRole(Context.LogonUser.Identity, "ADMIN"))
            {
				return;
			}
            if (Application.Roles.IsInRole(Context.LogonUser.Identity, "DEVELOPER"))
            {
				return;
			}
            if (Application.Roles.IsInRole(Context.LogonUser.Identity, "DATAMASTER"))
            {
				return;
			}
			throw new QorpentSecurityException("�� �� ������ ����� �� ������������ ��� ���������������� " +
			                                   Context.LogonUser.Identity.Name);
		}

		/// <summary>
		/// 	���������� �������� ������������
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			IPrincipal result = null;
			Target = (Target??"").Trim();
			if (Target.IsNotEmpty()) {
				result = new GenericPrincipal(new GenericIdentity(Target), null);
			}
			Application.Impersonation.Impersonate(Context.LogonUser, result);
			Application.Principal.SetCurrentUser(result ?? Context.LogonUser);
			return Application.Principal.CurrentUser.Identity.Name;
		}
	}
}