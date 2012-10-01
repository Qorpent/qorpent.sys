using System;
using System.Runtime.InteropServices;

namespace Qorpent.Security {
#if !Unix

	/// <summary>
	/// Access to native Windows API
	/// </summary>
	public static class NativeWindowsFunctions {
		/// <summary>
		/// Native logon to windows
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
	}

#endif
}