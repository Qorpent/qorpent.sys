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
// Original file : DefaultPrincipalSource.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Qorpent.IoC;
using Qorpent.Mvc;

namespace Qorpent.Security {
	/// <summary>
	/// 	Basic principal source implementation, 
	/// 	resolves principal or from QWebContext.Current or from Thread.CurrentPrincipal
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultPrincipalSource : ServiceBase, IPrincipalSource {
		/// <summary>
		/// </summary>
		[ThreadStatic] protected static IPrincipal Current;

#if PARANOID
		static DefaultPrincipalSource() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	Current user of thread/application
		/// </summary>
		public IPrincipal CurrentUser {
			get {
				lock (Sync) {
					if (null == Current) {
						if (IsWeb()) {
							if (null != MvcContextBase.Current && null != MvcContextBase.Current.LogonUser) {
								Current = MvcContextBase.Current.LogonUser;
							}
							else if (null!=Application.HttpWrapper.GetCurrentUser()) {
								Current = Application.HttpWrapper.GetCurrentUser();
							}
							else {
								Current = new GenericPrincipal(new GenericIdentity("local\\guest"), null);
							}
						}
						else {
							if (null != Thread.CurrentPrincipal && Thread.CurrentPrincipal.Identity.IsAuthenticated) {
								Current = Thread.CurrentPrincipal;
							}
							else {
								Current = new GenericPrincipal(new GenericIdentity(Environment.UserDomainName + "\\" + Environment.UserName),
								                               null);
							}
						}
					}

#if PARANOID
						if(Paranoid.Provider.IsSpecialUser(Current)) {
							if(!Paranoid.Provider.Authenticate(Current)) {
								throw new ParanoidException(ParanoidState.NotAuthSpecialUser);
							}
						}
#endif
					return Current;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IPrincipal BasePrincipal {
			get {
				if (IsWeb())
				{
					if (null != MvcContextBase.Current && null != MvcContextBase.Current.LogonUser)
					{
						return MvcContextBase.Current.LogonUser;
					}
					else if (null!=Application.HttpWrapper.GetCurrentUser())
					{
						return Application.HttpWrapper.GetCurrentUser();
					}
					else
					{
						return new GenericPrincipal(new GenericIdentity("local\\guest"), null);
					}
				}
				else
				{
					if (null != Thread.CurrentPrincipal && Thread.CurrentPrincipal.Identity.IsAuthenticated)
					{
						return Thread.CurrentPrincipal;
					}
					else
					{
						return new GenericPrincipal(new GenericIdentity(Environment.UserDomainName + "\\" + Environment.UserName),
													   null);
					}
				}
			}
			set {
				
			}
		}

		/// <summary>
		/// 	Manually set current user for thread
		/// </summary>
		/// <param name="usr"> </param>
		public void SetCurrentUser(IPrincipal usr) {
			lock (Sync) {
#if PARANOID
						if(Paranoid.Provider.IsSpecialUser(usr)) {
							if(!Paranoid.Provider.Authenticate(usr)) {
								throw new ParanoidException(ParanoidState.NotAuthSpecialUser);
							}
						}
#endif
				Log.Trace("current user changed to " + usr.Identity.Name);

				Current = usr;
			}
		}


		private bool IsWeb() {
			if (null == _isweb) {
				lock (Sync) {
					_isweb = EnvironmentInfo.IsWeb;
					if (null != Application) {
						_isweb = Application.IsWeb;
					}
				}
			}
			return _isweb.Value;
		}

		private bool? _isweb;
	}
}