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
// PROJECT ORIGIN: Qorpent.Mvc/DefaultActionAuthorizer.cs
#endregion
using System;
using Qorpent.IoC;
using Qorpent.Security;

namespace Qorpent.Mvc.Security {
	/// <summary>
	/// 	Default implementation of context authorizer
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class DefaultActionAuthorizer : ServiceBase, IActionAuthorizer {
		/// <summary>
		/// 	Retrieves autorization result against web request
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public AuthorizationResult Authorize(IMvcContext context) {
			lock (Sync) {
				if (null == context) {
					throw new ArgumentNullException("context");
				}
				if (null == context.LogonUser) {
					throw new ArgumentException("context.LogonUrl");
				}

#if PARANOID
				if(!((MvcContext)context).NativeASPContext.Request.IsSecureConnection) {
					return AuthorizationResult.Error(new ParanoidException(ParanoidState.NotSecureConnection));
				}
				if(!FormsAuthentication.IsEnabled) {
					return AuthorizationResult.Error(new ParanoidException(ParanoidState.NoFormAuthEnabled));
				}
				if(FormsAuthentication.LoginUrl!= "/"+Application.ApplicationName+ "/_sys/login.qview.qweb") {
					return AuthorizationResult.Error(new ParanoidException(ParanoidState.NoQorpentLoginUrl));
				}
				
				if (!(context.ActionDescriptor.Action.GetType()== typeof(LoginAction)) && !(context.ActionDescriptor.Action.GetType() == typeof(LogoutAction))) {
					if(!(context.LogonUser.Identity is FormsIdentity)) {
						return AuthorizationResult.Error(new ParanoidException(ParanoidState.NoFormAuthUsed));
					}
					if(Paranoid.Provider.IsSpecialUser(context.LogonUser)) {
						var coockie = ((MvcContext) context).NativeASPContext.Request.Cookies[FormsAuthentication.FormsCookieName];
						if(null==coockie) {
							throw new ParanoidException(ParanoidState.InvalidCookieSet);
						}
						try{
							Paranoid.Provider.CheckLogin(context.LogonUser,coockie.Value);
						}catch(ParanoidException){
							context.Redirect("_sys/login.qview.qweb?ReturnUrl="+context.Uri.PathAndQuery);
							return AuthorizationResult.OK;
						}
					}
				} 
#endif

				if ("local\\guest" == context.LogonUser.Identity.Name || "" == context.LogonUser.Identity.Name) {
					// guest - login only allowed

					if (context.ActionDescriptor.DirectRole == "DEFAULT" || context.ActionDescriptor.DirectRole == "") {
						return AuthorizationResult.OK;
					}
					return
						AuthorizationResult.Error(new QorpentSecurityException("гостевой доступ разрешен только для DEFAULT действий"));
				}
				var renderAuthorized = true;
				if (context.RenderDescriptor.UseAuthorization) {
					renderAuthorized = Application.Access.IsAccessible(context.RenderDescriptor, AccessRole.Execute);
				}
				if (!renderAuthorized) {
					return AuthorizationResult.Error(new Exception("render role checking against current user don't match"));
				}
				if (!(context.RenderDescriptor.OverrideActionAuthorization && context.RenderDescriptor.UseAuthorization)) {
					var roleresult = Application.Access.IsAccessible(context.ActionDescriptor, AccessRole.Execute);
					if (roleresult) {
						return AuthorizationResult.OK;
					}
				}
				else {
					return AuthorizationResult.OK;
				}
				return
					AuthorizationResult.Error(new QorpentSecurityException("action role checking against current user don't match"));
			}
		}
	}
}