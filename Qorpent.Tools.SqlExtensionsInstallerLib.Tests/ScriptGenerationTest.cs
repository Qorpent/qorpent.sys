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
// Original file : ScriptGenerationTest.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using Qorpent.Tools.SqlExtensionsInstallerLib;

namespace Qorpent.Tools.SqlExtensionsInstaller.Tests {
	public static class TestFunctions {
		[SqlFunction(
			IsDeterministic = false,
			SystemDataAccess = SystemDataAccessKind.None,
			DataAccess = DataAccessKind.None,
			Name = "testint")]
		public static SqlInt32 TestInt(SqlInt32 p1) {
			return 2;
		}
	}

	[TestFixture]
	public class ScriptGenerationTest {
		[SetUp]
		public void Setup() {
			var args = new SqlInstallerConsoleProgramArgs();
			args.Assembly = GetType().Assembly;
			args.GenerateScript = true;
			args.Schema = "test";
			_result = new SqlInstallerConsoleProgram().GenerateScripts(args);
		}


		private IEnumerable<string> _result;

		[Test]
		public void WillDropExisted() {
			Assert.True(_result.Any(x => x.Contains("DROP FUNCTION test.[testint]")));
		}
	}
}