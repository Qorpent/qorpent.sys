using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Qorpent.IoC;

namespace Qorpent.Security
{
	/// <summary>
	/// Allows dynamically logon to underlined system (for now Windows - only)
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton,ServiceType = typeof(ISysLogon))]
	public  class SysLogon : ISysLogon {
		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		public bool Logon(string username, string password, int logontype = WinLogonType.LOGON32_LOGON_NETWORK)
		{
			lock(this) {
				IntPtr token = new IntPtr();
				return Logon(username, password, ref token, logontype);
			}
		}

		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// can return system token of logon (windows only)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="token"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		public  bool Logon(string username, string password, ref IntPtr token, int logontype = WinLogonType.LOGON32_LOGON_NETWORK) {
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

				bool authenticated = NativeWindowsFunctions.LogonUserW(name, domain, password, logontype, 0, ref token);
				return authenticated;
#endif
			}
		}
	}
}
