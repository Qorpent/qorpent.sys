	namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// “екущее состо€ние параноидного режима
	/// </summary>
	public enum ParanoidState {
		/// <summary>
		/// No Qorpent.Security found
		/// </summary>
		NoAssembly,
		/// <summary>
		/// No Qorpent.Security.sygnature found
		/// </summary>
		NoAssemblySyg,
		/// <summary>
		/// Qorpent.Security sygnature is not real sygnature
		/// </summary>
		InvalidAssemblySygFormat,
		/// <summary>
		/// Qorpent.Security sygnature not verified with developer sign
		/// </summary>
		InvalidAssemblySyg,
		/// <summary>
		/// No passwd file found
		/// </summary>
		NoPasswd,
		/// <summary>
		/// No passwd sygnature file found
		/// </summary>
		NoPasswdSyg,
		/// <summary>
		/// passwd sygnature is not real sign
		/// </summary>
		InvalidPasswdSygFormat,
		/// <summary>
		/// passwd syg is not verified
		/// </summary>
		InvalidPasswdSyg,
		/// <summary>
		/// passwd file is invalid
		/// </summary>
		InvalidPasswdFormat,

		/// <summary>
		/// PARANOID ENVIRONMENT IS WELL
		/// </summary>
		Verified,
		/// <summary>
		/// Mark of innormal PARANOID start
		/// </summary>
		DoubleInitialize,
		/// <summary>
		/// Security assembly cannot be loaded
		/// </summary>
		InvalidDLL,
		/// <summary>
		/// —борка имеет неверную структуру
		/// </summary>
		InvalidDLLContent,
		/// <summary>
		/// Given paranoid type cannot be casted to IParanoidProvider
		/// </summary>
		InvalidProviderType,
		/// <summary>
		/// Something wrong with provider creation
		/// </summary>
		CannotCreateProvider,
		/// <summary>
		/// Not-defined error state
		/// </summary>
		GeneralError,
		/// <summary>
		/// Password file doesn't contains any SU user
		/// </summary>
		NoSuDefined,
		/// <summary>
		/// In console mode uses not trusted user with non-interactive to ask for username and password
		/// </summary>
		CannotDoSuLoginInConsole,
		/// <summary>
		/// Console login from not su
		/// </summary>
		NoSuUser,
		/// <summary>
		/// If special user trys get current, but not authenticated with Paranoid
		/// </summary>
		NotAuthSpecialUser,
		/// <summary>
		/// 
		/// </summary>
		NonMvcCall,
		/// <summary>
		/// 
		/// </summary>
		NonMatchedCookie,
		/// <summary>
		///		
		/// </summary>
		NoFormAuthEnabled,
		/// <summary>
		/// 
		/// </summary>
		NoQorpentLoginUrl,
		/// <summary>
		/// 
		/// </summary>
		NoFormAuthUsed,
		/// <summary>
		/// 
		/// </summary>
		NotSecureConnection,
		/// <summary>
		/// 
		/// </summary>
		InvalidCookieSet
	}
}