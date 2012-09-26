#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
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
// Original file : BxlParserOptions.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Set of flags to configure bxl parsing process
	/// </summary>
	[Flags]
	public enum BxlParserOptions {
		/// <summary>
		/// 	default zero option
		/// </summary>
		None = 0,

		/// <summary>
		/// 	do not generate lexinfo data in result XML (result size-optimization and readablitiy)
		/// </summary>
		NoLexData = 1,

		/// <summary>
		/// 	use '__' prefixed standard attributes (code,name,id) instead of direct names
		/// </summary>
		SafeAttributeNames = 2,

		/// <summary>
		/// 	prevent creation of 'code' attribute in id-code pare
		/// </summary>
		OnlyIdAttibute = 4,

		/// <summary>
		/// 	prevent creation of 'id' attribute in id-code pare
		/// </summary>
		OnlyCodeAttribute = 8,

		/// <summary>
		/// 	prevents values and elements generation - DEBUG propose only
		/// </summary>
		NoElements = 16,

		/// <summary>
		/// 	Forces remove ROOT element if only one child element
		/// </summary>
		ExtractSingle = 32
	}
}