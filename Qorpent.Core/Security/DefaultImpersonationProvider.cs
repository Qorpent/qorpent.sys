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
// Original file : DefaultImpersonationProvider.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	Стандартная реализация <see cref="IImpersonationProvider" />
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultImpersonationProvider : IImpersonationProvider {
		/// <summary>
		/// 	Регистриует имперсонацию одной учетной записи в другую
		/// </summary>
		/// <param name="srcUser"> </param>
		/// <param name="resultUser"> null - снимает имперсонацию </param>
		public void Impersonate(IPrincipal srcUser, IPrincipal resultUser) {
			lock (this) {
				var key = GetKey(srcUser);
				impersonationMap[key] = resultUser;
			}
		}

		/// <summary>
		/// 	True - указанная запись имеет имперсонированный эквивалент
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		public bool IsImpersonated(IPrincipal usr) {
			lock (this) {
				var key = GetKey(usr);
				if (!impersonationMap.ContainsKey(key)) {
					return false;
				}
				if (null == impersonationMap[key]) {
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 	Возвращает имперсонированную верисию учетной записи
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		public IPrincipal GetImpersonation(IPrincipal usr) {
			lock (this) {
				if (IsImpersonated(usr)) {
					return impersonationMap[GetKey(usr)];
				}
				return usr;
			}
		}


		private static string GetKey(IPrincipal usr) {
			var key = usr.Identity.Name.Replace("\\", "/").ToUpperInvariant();
			return key;
		}

		private readonly Dictionary<string, IPrincipal> impersonationMap = new Dictionary<string, IPrincipal>();
	}
}