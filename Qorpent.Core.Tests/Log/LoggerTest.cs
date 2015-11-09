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
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log {
	[TestFixture]
	public class LoggerTest {
		private class simplewriter : ILogWriter {
			public string Filter { get; set; }


		    public bool Active { get; set; } = true;

		    public void Write(LogMessage message) {
                if(!Active)return;
		        
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
		[Explicit]
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

	    class LateLogger : ILogger {
	        private string _message;
	        private Task _task;
	        public LogLevel Level { get; set; }
	        public bool Available { get; set; }
	        public string Name { get; set; }
            public string OutResult { get; set; }
	        public InternalLoggerErrorBehavior ErrorBehavior { get; set; }
	        public bool IsApplyable(object context) {
	            if (!Active) return false;
	            return true;
	        }

	        public void StartWrite(LogMessage message) {
	            if (!Active) return;
	            if (null != _task) _task.Wait();
	            this._message = message.Message;
	            this._task = Task.Run(() =>
	            {
                    Thread.Sleep(200);
	                OutResult = _message;
	            });
	        }

	        public void Join() {
               if(null!=_task)
	            _task.Wait();
	        }

	        public bool Active { get; set; } = true;
	    }
	    [Test]
	    public void UserLogIsSynchronized_Q206() {
	        var lateLogger = new LateLogger{Level = LogLevel.All,Available = true};
            IUserLog log = new LoggerBasedUserLog(new []{lateLogger}){Level = LogLevel.All};
            log.Trace("XXX");
            Assert.AreNotEqual("XXX",lateLogger.OutResult);//сначала проверяем, что так-то все асинхронно
            log.Synchronize();
            Assert.AreEqual("XXX", lateLogger.OutResult);//но можно дождаться
	        using (IUserLog slog = new LoggerBasedUserLog(new[] {lateLogger}) {Level = LogLevel.All}) {
	            slog.Trace("YYY");
	        }

            Assert.AreEqual("YYY",lateLogger.OutResult);
            
	    }
	}
}