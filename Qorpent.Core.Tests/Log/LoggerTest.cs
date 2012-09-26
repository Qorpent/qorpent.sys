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
// Original file : LoggerTest.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Threading;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log {
	[TestFixture]
	public class LoggerTest {
		private class simplewriter : ILogWriter {
			public string Filter { get; set; }


			public void Write(LogMessage message) {
				if (-1 == timeout) {
					throw new Exception("myerror");
				}
				Thread.Sleep(timeout); //emulate some working before flush
				called = true;
			}

			public LogLevel Level { get; set; }
			public bool called;
			public int timeout = 500;
		}


		[Test]
		public void CanThrowTimeOut() {
			var m = new simplewriter {timeout = 1050};
			var logger = new BaseLogger {Writers = new[] {m}};
			logger.StartWrite(new LogMessage());
			logger.Join();
			logger.StartWrite(new LogMessage());
			var ex = Assert.Throws<LogException>(() => logger.Join());
			StringAssert.Contains("timeout", ex.Message);
		}

		[Test]
		public void IsAsyncButSynchronizable() {
			var m = new simplewriter();
			var logger = new BaseLogger {Writers = new[] {m}};
			logger.StartWrite(new LogMessage());
			Assert.False(m.called); //we start write UserLog, but we are async
			logger.Join();
			Assert.True(m.called); //but here we are guaranted to be synchronized for next run

			// repeat it twice
			m.called = false;

			logger.StartWrite(new LogMessage());
			Assert.False(m.called); //we start write UserLog, but we are async
			logger.Join();
			Assert.True(m.called); //but here we are guaranted to be synchronized for next run
		}

		[Test]
		public void PopupInternalError() {
			var m = new simplewriter {timeout = -1};
			var logger = new BaseLogger {Writers = new[] {m}};
			logger.StartWrite(new LogMessage());

			var ex = Assert.Throws<LogException>(() => logger.Join());
			StringAssert.Contains("myerror", ex.InnerException.Message);
		}
	}
}