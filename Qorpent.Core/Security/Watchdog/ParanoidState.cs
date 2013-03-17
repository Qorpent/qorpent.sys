#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/ParanoidState.cs
#endregion
namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// 	Текущее состояние параноидного режима
	/// </summary>
	public enum ParanoidState {
		/// <summary>
		/// 	No Qorpent.Security found
		/// </summary>
		NoAssembly,

		/// <summary>
		/// 	No Qorpent.Security.sygnature found
		/// </summary>
		NoAssemblySyg,

		/// <summary>
		/// 	Qorpent.Security sygnature is not real sygnature
		/// </summary>
		InvalidAssemblySygFormat,

		/// <summary>
		/// 	Qorpent.Security sygnature not verified with developer sign
		/// </summary>
		InvalidAssemblySyg,

		/// <summary>
		/// 	No passwd file found
		/// </summary>
		NoPasswd,

		/// <summary>
		/// 	No passwd sygnature file found
		/// </summary>
		NoPasswdSyg,

		/// <summary>
		/// 	passwd sygnature is not real sign
		/// </summary>
		InvalidPasswdSygFormat,

		/// <summary>
		/// 	passwd syg is not verified
		/// </summary>
		InvalidPasswdSyg,

		/// <summary>
		/// 	passwd file is invalid
		/// </summary>
		InvalidPasswdFormat,

		/// <summary>
		/// 	PARANOID ENVIRONMENT IS WELL
		/// </summary>
		Verified,

		/// <summary>
		/// 	Mark of innormal PARANOID start
		/// </summary>
		DoubleInitialize,

		/// <summary>
		/// 	Security assembly cannot be loaded
		/// </summary>
		InvalidDll,

		/// <summary>
		/// 	Сборка имеет неверную структуру
		/// </summary>
		InvalidDllContent,

		/// <summary>
		/// 	Given paranoid type cannot be casted to IParanoidProvider
		/// </summary>
		InvalidProviderType,

		/// <summary>
		/// 	Something wrong with provider creation
		/// </summary>
		CannotCreateProvider,

		/// <summary>
		/// 	Not-defined error state
		/// </summary>
		GeneralError,

		/// <summary>
		/// 	Password file doesn't contains any SU user
		/// </summary>
		NoSuDefined,

		/// <summary>
		/// 	In console mode uses not trusted user with non-interactive to ask for username and password
		/// </summary>
		CannotDoSuLoginInConsole,

		/// <summary>
		/// 	Console login from not su
		/// </summary>
		NoSuUser,

		/// <summary>
		/// 	If special user trys get current, but not authenticated with Paranoid
		/// </summary>
		NotAuthSpecialUser,

		/// <summary>
		/// </summary>
		NonMvcCall,

		/// <summary>
		/// </summary>
		NonMatchedCookie,

		///<summary>
		///</summary>
		NoFormAuthEnabled,

		/// <summary>
		/// </summary>
		NoQorpentLoginUrl,

		/// <summary>
		/// </summary>
		NoFormAuthUsed,

		/// <summary>
		/// </summary>
		NotSecureConnection,

		/// <summary>
		/// </summary>
		InvalidCookieSet,

		/// <summary>
		/// 	Подмена IRoleResolver
		/// </summary>
		InvalidRoleResolver,

		/// <summary>
		/// 	Подмена IPrincipalSource
		/// </summary>
		InvalidPrincipalSource
	}
}