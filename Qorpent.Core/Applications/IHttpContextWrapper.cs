using System.Security.Principal;

namespace Qorpent.Applications {
	/// <summary>
	/// Special interface to remove dependency on System.Web from Qorpent.Core
	/// it's problem because SQL 2012 cannot load dependent code from System.Web
	/// </summary>
	public interface IHttpContextWrapper {
		/// <summary>
		/// Returns true if current http context exists
		/// </summary>
		/// <returns></returns>
		bool HasCurrent();
		/// <summary>
		/// Wraps current request application path
		/// </summary>
		/// <returns></returns>
		string GetCurrentApplicationPath();

		/// <summary>
		/// Wraps current principal of http context
		/// </summary>
		/// <returns></returns>
		IPrincipal GetCurrentUser();
		/// <summary>
		/// retrieves app domain path for httpruntime
		/// </summary>
		/// <returns></returns>
		string GetAppDomainAppPath();
	}
}