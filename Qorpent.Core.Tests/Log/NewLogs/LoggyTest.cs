using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace Qorpent.Core.Tests.Log.NewLogs
{
    [TestFixture]
    public class LoggyTest
    {
        private DefaultLoggy logger;
        private TestAppender appender;

        [SetUp]
        public void Setup() {
            this.logger = new DefaultLoggy();
            logger.Appenders.Add(this.appender = new TestAppender());

        }

        [Test]
        public void CanLogWithSpecialMethods() {
            logger.Debug("test");
            logger.Trace("test");
            logger.Info("test");
            logger.Warn("test");
            logger.Error("test");
            logger.Fatal("test");
        }

        [Test]
        public void CanUseWriters() {

            logger.Info("test");
            Assert.True(appender.Catched(LogLevel.Info, "test"));
        }

        [Test]
        public void ValidlyDetectsLevel() {
            logger.Level = LogLevel.Debug;

            Assert.True(logger.IsForInfo());
            Assert.False(logger.IsForTrace());
            appender.Level = LogLevel.Trace;
            Assert.True(logger.IsForTrace());
            logger.Level = LogLevel.Info;
            Assert.False(logger.IsForTrace());
            Assert.True(logger.IsForInfo());
        }

        [Test]
        public void ValidMessageParsing() {
            var ex = new Exception();
            var lm = new LoggyMessage(LogLevel.Error, "test", ex);
            Assert.AreEqual(LogLevel.Error,lm.Level);
            Assert.AreEqual("test",lm.Message);
            Assert.AreEqual(ex, lm.Exception);
        }
    }
}
