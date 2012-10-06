using System.Security.Principal;

namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// Provides paranoid mode environment 
	/// </summary>
	public interface IParanoidProvider {
		/// <summary>
		/// Indicates that all is well
		/// </summary>
		bool OK { get; }

		/// <summary>
		/// State of environment
		/// </summary>
		ParanoidState State { get; }
		/// <summary>
		/// Deteremine if User is under special control
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		bool IsSpecialUser(IPrincipal principal);
		/// <summary>
		/// Authenticate user on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		bool Authenticate(IPrincipal principal, string password);
		/// <summary>
		/// Determine user role on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		bool IsInRole(IPrincipal principal, string role);

		/// <summary>
		/// True if role is under Paranoid control
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		bool IsSecureRole (string role);

		/// <summary>
		/// Authenticate user on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		bool Authenticate(IPrincipal principal);
	}
}