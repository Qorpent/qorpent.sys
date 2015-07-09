using System.Net;
using qorpent.v2.security.user;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	/// 
	/// </summary>
	public class LogonInfo {
		public Identity Identity { get; set; }
		public IPEndPoint RemoteEndPoint { get; set; }
		public IPEndPoint LocalEndPoint { get; set; }
		public string UserAgent { get; set; }
	}
}