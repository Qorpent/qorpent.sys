namespace Qorpent.Security {
	/// <summary>
	/// Constants for win logon type (Windows only)
	/// </summary>
	public static class WinLogonType {
		/// <summary>
		/// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
		/// </summary>
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		/// <summary>
		/// This logon type is intended for high performance servers to authenticate plaintext passwords. The LogonUser function does not cache credentials for this logon type.
		/// </summary>
		public const int LOGON32_LOGON_NETWORK = 3;

		/// <summary>
		/// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers.
		/// </summary>
		public const int LOGON32_LOGON_BATCH = 4;

		/// <summary>
		/// Indicates a service-type logon. The account provided must have the service privilege enabled.
		/// </summary>
		public const int LOGON32_LOGON_SERVICE = 5;
		
		
	}
}