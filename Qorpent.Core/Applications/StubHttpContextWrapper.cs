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
// PROJECT ORIGIN: Qorpent.Core/StubHttpContextWrapper.cs
#endregion
using System;
using System.IO;
using System.Security.Principal;

namespace Qorpent.Applications {
	/// <summary>
	/// Stub implementation for non-web context
	/// </summary>
	public class StubHttpContextWrapper:IHttpContextWrapper {
		/// <summary>
		/// If no MVC - always false
		/// </summary>
		/// <returns></returns>
		public bool HasCurrent() {
			return false;
		}
		/// <summary>
		/// If no MVC - always null
		/// </summary>
		/// <returns></returns>
		public string GetCurrentApplicationPath() {
			return null;
		}

		/// <summary>
		/// Wraps current principal of http context
		/// </summary>
		/// <returns></returns>
		public IPrincipal GetCurrentUser() {
			return null;
		}

		/// <summary>
		/// retrieves app domain path for httpruntime
		/// </summary>
		/// <returns></returns>
		public string GetAppDomainAppPath() {
			var result = Environment.CurrentDirectory;
			if(EnvironmentInfo.IsWeb) {
				return Path.GetDirectoryName(result);
			}
			return result;
		}
	}
}