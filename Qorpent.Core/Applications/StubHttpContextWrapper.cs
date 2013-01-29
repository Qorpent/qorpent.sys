using System;
using System.Security.Principal;

namespace Qorpent.Applications {
	/// <summary>
	/// Stub implementation for non-web context
	/// </summary>
	public class StubHttpContextWrapper:IHttpContextWrapper {
		/// <summary>
		/// If no MVC - always false
		/// </summary>
		/// <returns></returns>
		public bool HasCurrent() {
			return false;
		}
		/// <summary>
		/// If no MVC - always null
		/// </summary>
		/// <returns></returns>
		public string GetCurrentApplicationPath() {
			return null;
		}

		/// <summary>
		/// Wraps current principal of http context
		/// </summary>
		/// <returns></returns>
		public IPrincipal GetCurrentUser() {
			return null;
		}

		/// <summary>
		/// retrieves app domain path for httpruntime
		/// </summary>
		/// <returns></returns>
		public string GetAppDomainAppPath() {
			return Environment.CurrentDirectory;
		}
	}
}