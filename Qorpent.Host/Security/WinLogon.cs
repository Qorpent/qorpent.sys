using System;
using System.Runtime.InteropServices;
using Qorpent.Security;

namespace Qorpent.Host.Security{
	// Shamelessly lifted from http://discuss.itacumens.com/index.php?topic=62872.0, 
	// then converted to C# (http://www.developerfusion.com/tools/convert/vb-to-csharp/) and
	// changed where necessary.


	internal class WinLogon{
		/// <summary>
		///     Native logon to windows
		/// </summary>
		/// <param name="lpszUsername"></param>
		/// <param name="lpszDomain"></param>
		/// <param name="lpszPassword"></param>
		/// <param name="dwLogonType"></param>
		/// <param name="dwLogonProvider"></param>
		/// <param name="phToken"></param>
		/// <returns></returns>
		[DllImport("ADVAPI32.dll", EntryPoint =
			"LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool LogonUserW(string lpszUsername, string lpszDomain,
		                                     string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		/// <summary>
		///     Execute system logon procedure and return true if proceed
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool Logon(string username, string password){
			lock (this){
				var token = new IntPtr();
				return Logon(username, password, ref token, WinLogonType.Logon32LogonNetwork);
			}
		}

		/// <summary>
		///     Execute system logon procedure and return true if proceed
		///     can return system token of logon (windows only)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="token"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		private bool Logon(string username, string password, ref IntPtr token,
		                   int logontype = WinLogonType.Logon32LogonNetwork){
			lock (this){
				string name = username;
				string domain = ".";
				username = username.Replace("\\", "/");
				if (username.Contains("/")){
					domain = username.Split('/')[0];
					name = username.Split('/')[1];
				}
				else if (username.Contains("@")){
					domain = username.Split('@')[1];
					name = username.Split('@')[0];
				}

				bool authenticated = LogonUserW(name, domain, password, logontype, 0, ref token);
				return authenticated;
			}
		}
	}
}