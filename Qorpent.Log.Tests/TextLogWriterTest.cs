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
// Original file : TextLogWriterTest.cs
// Project: Qorpent.Log.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log.Tests {
	[TestFixture]
	public class TextLogWriterTest {
		[Test]
		[Ignore("not sync, so result can be unexpected")]
		public void CanWriteSomeLogWithLevelChecking() {
			var man = new DefaultLogManager();
			var file = Path.GetTempFileName();
			var file2 = Path.GetTempFileName();
			var file3 = Path.GetTempFileName();
			var writer = new TextFileWriter {FileName = file, CustomFormat = "${Name}:${Level}", Level = LogLevel.Error};
			var writer2 = new TextFileWriter {FileName = file2, CustomFormat = "${Name}:${Level}", Level = LogLevel.Warning};
			var writer3 = new TextFileWriter {FileName = file3, CustomFormat = "${Name}:${Level}", Level = LogLevel.Trace};
			var logger = new BaseLogger
				{
					Writers = new[] {writer, writer2, writer3},
					Level = LogLevel.Info
					,
					Mask = "ZZZ"
				};
			var logger2 = new BaseLogger
				{
					Writers = new[] {writer, writer2, writer3},
					Level = LogLevel.Trace
					,
					Mask = "ZZZ2"
				};
			man.ErrorBehavior = InternalLoggerErrorBehavior.Throw;

			man.Loggers.Add(logger);
			man.Loggers.Add(logger2);

			var l1 = man.GetLog("ZZZ1", this);
			var l2 = man.GetLog("ZZZ2", this);
			var l3 = man.GetLog("XXX", this); //stub

			l1.Debug("");
			l2.Debug("");
			l3.Debug("");

			l1.Trace("");
			l2.Trace("");
			l3.Trace("");
			Thread.Sleep(10);
			l1.Info("");
			l2.Info("");
			l3.Info("");


			l1.Warn("");
			l2.Warn("");
			l3.Warn("");

			l1.Error("");
			l2.Error("");
			l3.Error("");

			man.Join(); //for direct file access

			var f1 = File.ReadAllText(file);
			var f2 = File.ReadAllText(file2);
			var f3 = File.ReadAllText(file3);
			Console.WriteLine(f1);
			Console.WriteLine(f2);
			Console.WriteLine(f3);
			File.Delete(file);
			File.Delete(file2);
			File.Delete(file3);
			Assert.AreEqual(@"ZZZ1:Error
ZZZ2:Error
ZZZ2:Error
".Trim().LfOnly(), f1.Trim().LfOnly());
			Assert.AreEqual(@"ZZZ1:Warning
ZZZ2:Warning
ZZZ2:Warning
ZZZ1:Error
ZZZ2:Error
ZZZ2:Error".Trim().LfOnly(), f2.Trim().LfOnly());
			Assert.AreEqual(@"ZZZ2:Trace
ZZZ1:Info
ZZZ2:Info
ZZZ2:Info
ZZZ1:Warning
ZZZ2:Warning
ZZZ2:Warning
ZZZ1:Error
ZZZ2:Error
ZZZ2:Error".Trim().LfOnly(), f3.Trim().LfOnly());
		}
	}
}