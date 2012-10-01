using System;

namespace Qorpent.Security {
	/// <summary>
	/// ѕозвол€ет осуществл€ть вход в систему от имени другого пользовател€ или провер€ть его валидность
	/// </summary>
	public interface ISysLogon {
		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		bool Logon(string username, string password, int logontype = WinLogonType.LOGON32_LOGON_NETWORK);

		/// <summary>
		/// Execute system logon procedure and return true if proceed
		/// can return system token of logon (windows only)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="token"></param>
		/// <param name="logontype"></param>
		/// <returns></returns>
		bool Logon(string username, string password, ref IntPtr token, int logontype = WinLogonType.LOGON32_LOGON_NETWORK);
	}
}