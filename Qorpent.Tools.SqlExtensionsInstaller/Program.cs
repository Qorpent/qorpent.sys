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
// Original file : Program.cs
// Project: Qorpent.Tools.SqlExtensionsInstaller
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Tools.SqlExtensionsInstallerLib;
using Qorpent.Utils;

namespace Qorpent.Tools.SqlExtensionsInstaller {
	/// <summary>
	/// 	runs sqlinstaller
	/// </summary>
	public static class Program {
		/// <summary>
		/// 	main method for Executable
		/// </summary>
		/// <param name="args"> </param>
		/// <returns> </returns>
		public static int Main(string[] args) {
			try {
				var strongTypedArguments = new ConsoleArgumentHelper().Parse<SqlInstallerConsoleProgramArgs>(args);
				return new SqlInstallerConsoleProgram().Run(strongTypedArguments);
			}
			catch (Exception ex) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
				return -1;
			}
		}
	}
}