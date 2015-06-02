using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace Qorpent.Core.Tests.Log.NewLogs {
    public class TestAppender:ILogAppender {
        
        public bool Catched(LogLevel level, string test, string loggername = null) {
            var key = level + ":" + test;
            if (!string.IsNullOrWhiteSpace(loggername)) {
                key += ":" + loggername;
            }
            return Messages.Contains(key);
        }

        public void MustCatch(LogLevel level, string test, string loggername = null) {
            Assert.True(Catched(level,test,loggername));
        }
        public void MustNotCatch(LogLevel level, string test, string loggername = null)
        {
            Assert.False(Catched(level, test, loggername));
        }

        public void Dispose() {
            
        }
        public IList<string> Messages = new List<string>();

        public void Write(LoggyMessage message) {
            var key = message.Level + ":" + message.Message;
            if (!string.IsNullOrWhiteSpace(message.LoggerName))
            {
                key += ":" + message.LoggerName;
            }
            Messages.Add(key);
        }

        public void Flush() {
        }

        public LogLevel Level { get; set; }
    }
}