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
// Original file : DefaultLogManagerTest.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log {
	[TestFixture]
	public class DefaultLogManagerTest {
		private DefaultLogManager createLogger(params string[] supports) {
			var result = new DefaultLogManager();
			foreach (var support in supports) {
				var logger = new BaseLogger {Mask = support};
				result.Loggers.Add(logger);
			}
			return result;
		}

		[TestCase("NOTEST")]
		[TestCase("")]
		public void ReturnsStubOnEmptyMatch(string type) {
			var man = createLogger("^TEST");
			var logl = man.GetLog(type, null);
			Assert.IsInstanceOf<StubUserLog>(logl);
		}

		[TestCase("TES.")]
		[TestCase("TEST")]
		[TestCase(".EST")]
		[TestCase("*")]
		public void ReturnsLoggersOnEmptyMatch(string mask) {
			var man = createLogger(mask);
			var logl = man.GetLog("TEST", null);
			Assert.IsInstanceOf<LoggerBasedUserLog>(logl);
		}

		[Test]
		public void FailSafeWorks() {
			var file = Path.Combine(EnvironmentInfo.RootDirectory, "failsafelog0.txt");
			if (File.Exists(file)) {
				File.Delete(file);
			}
			var man = new DefaultLogManager();
			man.LogFailSafe(new LogMessage {Message = "FailSafeWorks"});
			var teststr = File.ReadAllText(file);
			Console.WriteLine(teststr);
			StringAssert.Contains("FailSafeWorks", teststr);
		}
	}
}