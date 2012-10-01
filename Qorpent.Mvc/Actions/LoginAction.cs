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
// Original file : LoginAction.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Web.Security;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Login simple action
	/// </summary>
	[Action("_sys.login", Help = "EN: used to authenticate with form based method", Role = "DEFAULT")]
	public class LoginAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			if (login.IsNotEmpty()) {
				var authenticator = Context.Application.Container.Get<IFormAuthenticationProvider>() ??
				                    new TestFormAuthenticationProvider();
				if (FormsAuthentication.Authenticate(login, pass) ||
				   authenticator.IsAuthenticated(login, pass, Context)) {
					FormsAuthentication.RedirectFromLoginPage(login, false);
					return new {needform = false, login};
				}
			}
			return new {needform = true, login};
		}

		/// <summary>
		/// </summary>
		[Bind(Name = "_l_o_g_i_n_", Required = false, ValidatePattern = @"^[\w\.-\\]+$")] protected string login;

		/// <summary>
		/// </summary>
		[Bind(Name = "_p_a_s_s_", Required = false, ValidatePattern = @"^[\w\S]{2,}$")] protected string pass;
	}
}