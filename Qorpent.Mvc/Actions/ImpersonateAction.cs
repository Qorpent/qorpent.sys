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
// Original file : ImpersonateAction.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Security.Principal;
using Qorpent.Mvc.Binding;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Вызывает регистрацию записи об имперсонации текущей (Logon) учетной записи  - дефолтная роль DEFAULT - реальная проверка
	/// 	внтури по Logon
	/// </summary>
	[Action("_sys.impersonate", Role = "DEFAULT", Help = "Позволяет имитировать вход от имени другого пользователя")]
	public class ImpersonateAction : ActionBase {
		/// <summary>
		/// 	Целевая учетная запись
		/// </summary>
		[Bind] public string Target { get; set; }


		/// <summary>
		/// 	4 part of execution - all internal context is ready - authorize it with custom logic
		/// </summary>
		protected override void Authorize() {
			if (Target.IsEmpty()) {
				return;
			}
			if (Application.Roles.IsInRole(Context.LogonUser, "ADMIN")) {
				return;
			}
			if (Application.Roles.IsInRole(Context.LogonUser, "DEVELOPER")) {
				return;
			}
			if (Application.Roles.IsInRole(Context.LogonUser, "DATAMASTER")) {
				return;
			}
			throw new QorpentSecurityException("Вы не имеете право на имперсонацию или переимперсонацию " +
			                                   Context.LogonUser.Identity.Name);
		}

		/// <summary>
		/// 	Собственно вызывает имперсонацию
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			IPrincipal result = null;
			if (Target.IsNotEmpty()) {
				result = new GenericPrincipal(new GenericIdentity(Target), null);
			}
			Application.Impersonation.Impersonate(Context.LogonUser, result);
			Application.Principal.SetCurrentUser(result ?? Context.LogonUser);
			return Application.Principal.CurrentUser.Identity.Name;
		}
	}
}