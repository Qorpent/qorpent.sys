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
// PROJECT ORIGIN: Qorpent.Core/IHttpContextWrapper.cs
#endregion
using System.Security.Principal;

namespace Qorpent.Applications {
	/// <summary>
	/// Special interface to remove dependency on System.Web from Qorpent.Core
	/// it's problem because SQL 2012 cannot load dependent code from System.Web
	/// </summary>
	public interface IHttpContextWrapper {
		/// <summary>
		/// Returns true if current http context exists
		/// </summary>
		/// <returns></returns>
		bool HasCurrent();
		/// <summary>
		/// Wraps current request application path
		/// </summary>
		/// <returns></returns>
		string GetCurrentApplicationPath();

		/// <summary>
		/// Wraps current principal of http context
		/// </summary>
		/// <returns></returns>
		IPrincipal GetCurrentUser();
		/// <summary>
		/// retrieves app domain path for httpruntime
		/// </summary>
		/// <returns></returns>
		string GetAppDomainAppPath();
	}
}