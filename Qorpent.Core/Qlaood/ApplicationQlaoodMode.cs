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
// Original file : ApplicationQlaoodMode.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Mode of domain/application along qlaood
	/// </summary>
	[Flags]
	public enum ApplicationQlaoodMode {
		/// <summary>
		/// 	Initial state before checking
		/// </summary>
		Undefined = 0,

		/// <summary>
		/// 	No qlaood dll found, not subdomain of qlaood application
		/// </summary>
		None = 1,

		/// <summary>
		/// 	Qlaood dll found in working directory, but not activated
		/// </summary>
		Available = 2,

		/// <summary>
		/// 	Current domain is hosted on Qlaood host
		/// </summary>
		Hosted = 4,

		/// <summary>
		/// 	Current domain is active host of Qlaood
		/// </summary>
		Host = 8,
	}
}