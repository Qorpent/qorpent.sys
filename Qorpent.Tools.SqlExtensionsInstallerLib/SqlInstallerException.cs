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
// Original file : SqlInstallerException.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	/// <summary>
	/// 	General error of SqlInstaller
	/// </summary>
	[Serializable]
	public class SqlInstallerException : Exception {
		/// <summary>
		/// 	creates new instance with message and possible inner exception
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="inner"> </param>
		public SqlInstallerException(string message, Exception inner = null) : base(message, inner) {}
	}
}