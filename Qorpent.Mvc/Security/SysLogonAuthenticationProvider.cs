﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Mvc/SysLogonAuthenticationProvider.cs
#endregion
using Qorpent.Applications;

namespace Qorpent.Mvc.Security {
	/// <summary>
	/// 	Simple TEST-ONLY-PROPOSE form authentication provider for Qorpent
	/// </summary>
	public class SysLogonAuthenticationProvider : IFormAuthenticationProvider {
		/// <summary>
		/// 	True if given context is authenticate - only admin/admin and user/user in LocalHost context will be authenticated
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="password"> </param>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public bool IsAuthenticated(string name, string password, IMvcContext context) {
#if !Unix
			if (Application.Current.SysLogon != null) {
				return Application.Current.SysLogon.Logon(name, password);
			}
			return false;
#else
			var key = name + ":" + password;
			if (null == context) {
				return false;
			}
			if (key != "admin:admin" && key != "user:user") {
				return false;
			}
			if (context.IsLocalHost()) {
				return true;
			}
			return false;
#endif
		}
	}
}