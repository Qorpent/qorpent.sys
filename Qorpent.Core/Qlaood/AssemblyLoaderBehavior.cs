#region LICENSE

// Copyright 2007-2013 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : AssemblyLoaderBehavior.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Behavior options of assembly loader
	/// </summary>
	[Flags]
	public enum AssemblyLoaderBehavior {
		/// <summary>
		/// 	By default - not found assembly returned as null, pdb loaded in binary mode if exists
		/// </summary>
		Default = ErrorOnInnerException | LoadSymbolInfo,

		/// <summary>
		/// 	None behavior options, Default will be used in method
		/// </summary>
		None = 0,

		/// <summary>
		/// 	Null will return on any errors occured
		/// </summary>
		ErrorSafe = 1,

		/// <summary>
		/// 	Not found of assembly will cause error
		/// </summary>
		ErrorOnNotFound = 2,

		/// <summary>
		/// 	Initialization/loader exceptions will be rethrown
		/// </summary>
		ErrorOnInnerException = 4,

		/// <summary>
		/// 	If choosed, loader will try supply Pdb/Mdb File along with assembly (for binary mode)
		/// </summary>
		LoadSymbolInfo = 8,
	}
}