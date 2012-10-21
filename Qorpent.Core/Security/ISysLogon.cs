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
// Original file : ISysLogon.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Security {
	/// <summary>
	/// 	ѕозвол€ет осуществл€ть вход в систему от имени другого пользовател€ или провер€ть его валидность
	/// </summary>
	public interface ISysLogon {
		/// <summary>
		/// 	Execute system logon procedure and return true if proceed
		/// </summary>
		/// <param name="username"> </param>
		/// <param name="password"> </param>
		/// <param name="logontype"> </param>
		/// <returns> </returns>
		bool Logon(string username, string password, int logontype = WinLogonType.Logon32LogonNetwork);

		/// <summary>
		/// 	Execute system logon procedure and return true if proceed
		/// 	can return system token of logon (windows only)
		/// </summary>
		/// <param name="username"> </param>
		/// <param name="password"> </param>
		/// <param name="token"> </param>
		/// <param name="logontype"> </param>
		/// <returns> </returns>
		bool Logon(string username, string password, ref IntPtr token, int logontype = WinLogonType.Logon32LogonNetwork);
	}
}