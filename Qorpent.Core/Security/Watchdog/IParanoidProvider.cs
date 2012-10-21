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
// Original file : IParanoidProvider.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Security.Principal;

namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// 	Provides paranoid mode environment
	/// </summary>
	public interface IParanoidProvider {
		/// <summary>
		/// 	Indicates that all is well
		/// </summary>
		bool OK { get; }

		/// <summary>
		/// 	State of environment
		/// </summary>
		ParanoidState State { get; }

		/// <summary>
		/// 	Deteremine if User is under special control
		/// </summary>
		/// <param name="principal"> </param>
		/// <returns> </returns>
		bool IsSpecialUser(IPrincipal principal);

		/// <summary>
		/// 	Authenticate user on custom way
		/// </summary>
		/// <param name="principal"> </param>
		/// <param name="password"> </param>
		/// <returns> </returns>
		bool Authenticate(IPrincipal principal, string password);

		/// <summary>
		/// 	Determine user role on custom way
		/// </summary>
		/// <param name="principal"> </param>
		/// <param name="role"> </param>
		/// <returns> </returns>
		bool IsInRole(IPrincipal principal, string role);

		/// <summary>
		/// 	True if role is under Paranoid control
		/// </summary>
		/// <param name="role"> </param>
		/// <returns> </returns>
		bool IsSecureRole(string role);

		/// <summary>
		/// 	Authenticate user on custom way
		/// </summary>
		/// <param name="principal"> </param>
		/// <returns> </returns>
		bool Authenticate(IPrincipal principal);

		/// <summary>
		/// </summary>
		/// <param name="login"> </param>
		/// <param name="coockievalue"> </param>
		void RegisterLogin(string login, string coockievalue);

		/// <summary>
		/// </summary>
		/// <param name="login"> </param>
		/// <param name="coockievalue"> </param>
		void RemoveLogin(string login, string coockievalue);

		/// <summary>
		/// </summary>
		/// <param name="principal"> </param>
		/// <param name="cookie"> </param>
		/// <exception cref="ParanoidException"></exception>
		void CheckLogin(IPrincipal principal, string cookie);
	}
}