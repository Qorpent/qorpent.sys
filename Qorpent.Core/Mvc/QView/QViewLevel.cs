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
// Original file : QViewLevel.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Level of QView  resolution
	/// </summary>
	public enum QViewLevel {
		/// <summary>
		/// 	Base, described in code directly
		/// </summary>
		Code,

		/// <summary>
		/// 	Root of application
		/// </summary>
		Root,

		/// <summary>
		/// 	System-defined
		/// </summary>
		Sys,

		/// <summary>
		/// 	Edition-defined
		/// </summary>
		Mod,

		/// <summary>
		/// 	Usr-defined
		/// </summary>
		Usr
	}
}