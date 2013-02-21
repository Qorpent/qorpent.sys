#region LICENSE

// Copyright 2007-2013 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
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
// Original file : ApplicationHttpContextWrapper.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Security.Principal;
using System.Web;
using Qorpent.Applications;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Implementation of IApplicationHttpContexWrapper
	/// </summary>
	public class HttpContextWrapper : IHttpContextWrapper {
		/// <summary>
		/// Returns true if current http context exists
		/// </summary>
		/// <returns></returns>
		public bool HasCurrent() {
			return null != HttpContext.Current;
		}

		/// <summary>
		/// Wraps current request application path
		/// </summary>
		/// <returns></returns>
		public string GetCurrentApplicationPath() {
			if (!HasCurrent()) {
				return null;
			}
			try {
				return HttpContext.Current.Request.ApplicationPath;
			}catch(Exception) {
				return "unknown";
			}
		}

		/// <summary>
		/// Wraps current principal of http context
		/// </summary>
		/// <returns></returns>
		public IPrincipal GetCurrentUser() {
			if(!HasCurrent()) return null;
			return HttpContext.Current.User;
		}

		/// <summary>
		/// retrieves app domain path for httpruntime
		/// </summary>
		/// <returns></returns>
		public string GetAppDomainAppPath() {
			return HttpRuntime.AppDomainAppPath;
		}
	}
}