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
// Original file : FileWriterTest.cs
// Project: Qorpent.Log.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using NUnit.Framework;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.TestSupport;

namespace Qorpent.Log.Tests {
	[TestFixture]
	[QorpentFixture(UseTemporalFileSystem = true)]
	public class FileWriterTest : QorpentFixture {
		[Test]
		public void CanGenerateAdditionalFilesOnSizeChange() {
			var writer = new TextFileWriter
				{
					FileName = Path.Combine(Tmpdir, "my.log"),
					CustomFormat = "${Time} : ${Message}",
					Level = LogLevel.All,
					MaxSize = 0.5m, //500 байт
				};

			var message1 = new LogMessage
				{Time = new DateTime(2010, 5, 2), HostObject = "host1", Level = LogLevel.Warn, Message = "this is a message"};
			for (var i = 0; i <= 10; i++) {
				writer.Write(message1);
			}
			message1.Message = "another message";
			for (var i = 0; i <= 10; i++) {
				writer.Write(message1);
			}

			Assert.True(File.Exists(Path.Combine(Tmpdir, "0001.my.log")));
			Assert.True(File.Exists(Path.Combine(Tmpdir, "0002.my.log")));

			var fstlog = File.ReadAllText(Path.Combine(Tmpdir, "0001.my.log"));
			var seclog = File.ReadAllText(Path.Combine(Tmpdir, "0002.my.log"));

			var size1 = new FileInfo(Path.Combine(Tmpdir, "0001.my.log")).Length;
			var size2 = new FileInfo(Path.Combine(Tmpdir, "0002.my.log")).Length;

			Console.WriteLine(fstlog);
			Console.WriteLine(seclog);

			Console.WriteLine(size1);
			Console.WriteLine(size2);
			Assert.LessOrEqual(519, size1);
			Assert.LessOrEqual(345, size2);
		}

		[Test]
		public void CanSplitFileWithTemplatedName() {
			var writer = new TextFileWriter
				{
					FileName = Path.Combine(Tmpdir, "${date}-${host}-${level}.log"),
					CustomFormat = "${Time} : ${Message}",
					Level = LogLevel.All
				};

			var message1 = new LogMessage
				{Time = new DateTime(2010, 5, 2), HostObject = "host1", Level = LogLevel.Warn, Message = "message1"};
			writer.Write(message1);

			message1 = new LogMessage
				{Time = new DateTime(2010, 5, 2), HostObject = "host1", Level = LogLevel.Warn, Message = "message1_2"};
			writer.Write(message1);
			var message2 = new LogMessage
				{Time = new DateTime(2010, 5, 3), HostObject = "host2", Level = LogLevel.Error, Message = "message2"};
			writer.Write(message2);
			message2 = new LogMessage
				{Time = new DateTime(2010, 5, 3), HostObject = "host2", Level = LogLevel.Error, Message = "message2_1"};
			writer.Write(message2);
           
            

			Assert.True(File.Exists(Path.Combine(Tmpdir, "2010-05-02-host1-Warn.log")));
			Assert.True(File.Exists(Path.Combine(Tmpdir, "2010-05-03-host2-Error.log")));

			var fstlog = File.ReadAllText(Path.Combine(Tmpdir, "2010-05-02-host1-Warn.log"));
			var seclog = File.ReadAllText(Path.Combine(Tmpdir, "2010-05-03-host2-Error.log"));

			Console.WriteLine(fstlog);
			Console.WriteLine(seclog);

			Assert.AreEqual(@"2010-05-02 00:00:00 : message1
2010-05-02 00:00:00 : message1_2".LfOnly(), fstlog.Trim().LfOnly());
			Assert.AreEqual(@"2010-05-03 00:00:00 : message2
2010-05-03 00:00:00 : message2_1".LfOnly(), seclog.Trim().LfOnly());
		}
	}
}