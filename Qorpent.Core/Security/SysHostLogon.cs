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
// PROJECT ORIGIN: Qorpent.Security/SysLogon.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Qorpent.IoC;

namespace Qorpent.Security
{

	/// <summary>
	/// Allows dynamically logon to underlined system (for now Windows - only)
	/// </summary>
	[ContainerComponent(Lifestyle.Extension,ServiceType = typeof(IHostLogon),Name="win.logon")]
	public  class SysHostLogon : IHostLogon {

	    public SysHostLogon() {
	        Idx = 10000;
	    }
		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		public bool Logon(string username, string password, int logontype = WinLogonType.Logon32LogonNetwork)
		{
			lock(this) {
				IntPtr token = new IntPtr();
				return Logon(username, password, ref token, logontype);
			}
		}

        [DllImport("ADVAPI32.dll", EntryPoint =
            "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUserW(string lpszUsername, string lpszDomain,
                                             string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// can return system token of logon (windows only)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="token"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		public  bool Logon(string username, string password, ref IntPtr token, int logontype = WinLogonType.Logon32LogonNetwork) {
			lock(this) {
#if Unix
			throw new NotImplementedException("Not implemented for Unix now");
#else
				var name = username;
				var domain = ".";
				username = username.Replace("\\", "/");
				if (username.Contains("/")) {
					domain = username.Split('/')[0];
					name = username.Split('/')[1];
				}
				else if (username.Contains("@")) {
					domain = username.Split('@')[1];
					name = username.Split('@')[0];
				}

				bool authenticated = LogonUserW(name, domain, password, logontype, 0, ref token);
				return authenticated;
#endif
			}
		}

	    public bool IsAuth(string username, string password, IScope scope= null) {
	        return Logon(username, password, 3);
	    }

	    public IIdentity Logon(string username, string password,IScope scope = null) {
	        lock (this) {
	            var token = new IntPtr();
	            var auth = Logon(username, password, ref token, 3);
	            if (!auth) return null;
	            return new WindowsIdentity(token);
	        }
	    }

	    public int Idx { get; set; }
	}
}
