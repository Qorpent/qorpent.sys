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
// Original file : DefaultRoleResolver.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Security.Watchdog;

namespace Qorpent.Security {
	/// <summary>
	/// 	Base role resolver implementation. 
	/// 	1) For roles DEFAULT and GUEST it ALWAYS return TRUE
	/// 	2) For local accounts (treat that URL host is LOCALHOST) it returns TRUE (locals are admins policy)
	/// 	3) For all other principals it returns NATIVE IsInRole
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	[RequireReset(Role = "DEVELOPER",All = true,Options = new[]{"roles"})]
	public class DefaultRoleResolver : ServiceBase, IRoleResolver
	{
#if PARANOID
		static DefaultRoleResolver() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	Real role resolvers to be used
		/// </summary>
		[Inject] public IRoleResolverExtension[] Extensions { get; set; }


		/// <summary>
		/// 	Test given principal against role
		/// </summary>
		/// <param name="principal"> </param>
		/// <param name="role"> </param>
		/// <param name="exact"> </param>
		/// <param name="callcontext"> </param>
		/// <param name="customcontext"> </param>
		/// <returns> </returns>
		public bool IsInRole(IPrincipal principal, string role, bool exact = false, IMvcContext callcontext = null,
		                     object customcontext = null) {
			lock (this) {
#if PARANOID
				bool isadmin =  Paranoid.Provider.IsInRole(principal, "ADMIN");
				if(Paranoid.Provider.IsSecureRole(role)) {
					var presult = Paranoid.Provider.IsInRole(principal, role);
					if(presult)return presult;
					if(!exact){
						return isadmin;
					}
					return false;
				}	
				if(!exact){
					return isadmin;
				}
					
#endif
			

				var result = false;
				var cachekey = principal.Identity.Name + ";" + role + ";" + exact+";"+customcontext;
				Log.Debug("start check " + cachekey, this);
				if (_cache.ContainsKey(cachekey)) {
					result = _cache[cachekey];
					Log.Debug("cache " + cachekey + " " + result, this);
					return result;
				}
				result = EvaluateIsInRole(principal, role, exact, callcontext, customcontext);
				Log.Debug("result " + cachekey + " " + result, this);
				_cache[cachekey] = result;
				return result;
			}
		}

		/// <summary>
		/// 	Test given username against role
		/// </summary>
		/// <param name="username"> </param>
		/// <param name="role"> </param>
		/// <param name="exact"> </param>
		/// <param name="callcontext"> </param>
		/// <param name="customcontext"> </param>
		/// <returns> </returns>
		public bool IsInRole(string username, string role, bool exact = false, IMvcContext callcontext = null,
		                     object customcontext = null) {
			return IsInRole(new GenericPrincipal(new GenericIdentity(username), new string[] {}), role, exact, callcontext,
			                customcontext);
		}


		private bool EvaluateIsInRole(IPrincipal principal, string role, bool exact, IMvcContext callcontext,
		                              object customcontext) {
			callcontext = callcontext ?? MvcContextBase.Current;
			//ALL ARE DEFAULTS AND GUESTS
			if (InternalIsInRole(principal, role, callcontext, exact)) {
				return true;
			}

			if (null != Extensions) {
				foreach (var ext in Extensions) {
					var result = ext.IsInRole(principal, role, exact, callcontext, customcontext);
					if (result) {
						return true;
					}
				}
			}

			return principal.IsInRole(role);
		}

		private static bool InternalIsInRole(IPrincipal principal, string role, IMvcContext callcontext, bool exact) {
			if (role == "DEFAULT" || role == "GUEST" || string.IsNullOrWhiteSpace(role)) {
				return true;
			}


			//OTHERWISE ALL MUST BE AUTHENTICATED (IF NOT GENERIC)
			if (!(principal.Identity is GenericIdentity)) {
				if (!principal.Identity.IsAuthenticated) {
					return true;
				}
			}
			//HACK TO USE SHORT ADMIN NAME IN TEST ENVIRONMENT FOR ALL ROLES
			if (!exact && principal.Identity.Name == "admin") {
				return true;
			}

			//HACK TO USE SHORT ADMIN NAME IN TEST ENVIRONMENT FOR ALL ROLES
			if (!exact && principal.Identity.Name == "local\\MOD_MONO_ROOT")
			{
				return true;
			}


			//HACK FOR QUICK TEST AGAINST "local web admin"
			if (null != callcontext) {
				if (callcontext.IsLocalHost()) {
					var windowsIdentity = WindowsIdentity.GetCurrent();
					if (windowsIdentity != null && principal.Identity.Name.ToLower() == windowsIdentity.Name.ToLower()) {
						{
							return !exact;
						}
					}
				}
			}
			return false;
		}

		private readonly IDictionary<string, bool> _cache = new Dictionary<string, bool>();


		/// <summary>
		/// Сбрасывает кэш ролей и вызывает очистку у всех расширений
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override object Reset(ResetEventData data) {
			var _cachesize = _cache.Count;
			IList<string> _resetedextensions = new List<string>();
			base.Reset(data);
			foreach (var extension in Extensions) {
				if(extension is IResetable) {
					((IResetable) extension).Reset(data);
					_resetedextensions.Add(extension.GetType().Name);
				}
			}
			return new {dropped = _cachesize, reseted = _resetedextensions.ToArray()};
		}

		/// <summary>
		/// 	Возващает объект, описывающий состояние до очистки
		/// </summary>
		/// <returns> </returns>
		public override object GetPreResetInfo() {
			return _cache.Count;
		}
	}
}