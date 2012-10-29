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

using System;
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
			if (Login.IsNotEmpty()) {
				try {
					var plogin = Login;
					if (Login.ToUpper().StartsWith(Environment.MachineName.ToUpper() + "\\")) {
						plogin = "local\\" + Login.Split('\\')[1];
					}
#if PARANOID
				
				var principal = new GenericPrincipal(new GenericIdentity(plogin), null);
				if((plogin.StartsWith("qorpent-sys\\") || Paranoid.Provider.IsSpecialUser(principal)) && !Paranoid.Provider.Authenticate(principal,pass)) {
					return new { needform = true, login };		
				}
				bool authenticated = true;			
			
#else
#endif
#if PARANOID
				if (!login.StartsWith("qorpent-sys\\")) {
#endif
					var authenticator = Context.Application.Container.Get<IFormAuthenticationProvider>() ??
					                    new SysLogonAuthenticationProvider();
					bool authenticated = authenticator.IsAuthenticated(Login, Pass, Context);
#if PARANOID
				}
#endif
					if (authenticated) {
						FormsAuthentication.SetAuthCookie(plogin, false);
#if PARANOID
				if (Paranoid.Provider.IsSpecialUser(principal))
					{
						

						var coockie = Context.ResponseCookies[FormsAuthentication.FormsCookieName];
						Paranoid.Provider.RegisterLogin(plogin, coockie.Value);
					}	
#endif


						var url = RetUrl;
						if (url.IsEmpty()) {
							url = FormsAuthentication.DefaultUrl;
						}
						return new {authenticated = true, login = plogin, returl=url};
					}

				}catch(Exception ex) {
					return new {authenticated = false, login = Login, errortype = ex.GetType().Name, errormessage = ex.Message};
				}
				return
					new
						{
							authenticated = false,
							login = Login,
							errortype = "unknown",
							errormessage = "Sys auth not proceed, in most cases it's because password problem"
						};
			}
			if(Context.LogonUser.Identity.Name=="" || Context.LogonUser.Identity.Name=="guest") {
				return new {authenticated = false, login = "", errormessage = "Не указан логин"};
			}else {
				return new { authenticated = true, login = Context.LogonUser.Identity.Name, user=Context.User.Identity.Name};
			}
		}

		/// <summary>
		/// </summary>
		[Bind(Name = "_l_o_g_i_n_", Required = false, ValidatePattern = @"^[\w\.-\\]+$")] protected string Login;

		/// <summary>
		/// </summary>
		[Bind(Name = "_p_a_s_s_", Required = false, ValidatePattern = @"^[\w\S]{2,}$")] protected string Pass;

		/// <summary>
		/// </summary>
		[Bind(Name = "ReturnUrl", Required = false)] protected string RetUrl;

	}
}