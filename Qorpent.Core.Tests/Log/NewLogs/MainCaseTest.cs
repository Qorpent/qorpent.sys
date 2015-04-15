using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log.NewLogs
{
    [TestFixture]
    public class MainCaseTest
    {
        private TestAppender appender;

        

        [Test]
        public void WriteToDefault() {
            Loggy.Info("test");
            appender.MustCatch(LogLevel.Info, "test", "default");
        }

        [Test]
        public void WriteToNamed() {
            var myloggy = Loggy.Get("named");
            myloggy.Info("test");
            appender.MustCatch(LogLevel.Info, "test", "named");
        }

        [Test]
        public void WriteToSpecial() {
            var myappender = new TestAppender {Level = LogLevel.Debug};
            Loggy.Get("named", s => {
                s.Isolated = true;
                s.Level = LogLevel.Info;
                s.Appenders.Add(myappender);
            });
            Loggy.Get("named").Info("test");
            appender.MustNotCatch(LogLevel.Info,"test","named");
            myappender.MustCatch(LogLevel.Info,"test","named");
        }

        [Test]
        public void CanResetLogger() {
            var logger = Loggy.Get("named");
            Assert.False(logger.Isolated);
            Loggy.Manager.Loggers["named"] = new DefaultLoggy {Name = "xxxx"};
            var logger2 = Loggy.Get("named");
            Assert.AreEqual("xxxx",logger2.Name);
        }

        [Test]
        public void ValidLevelDetection()
        {
            Assert.True(Loggy.IsForInfo());
            Assert.False(Loggy.IsForTrace());
        }

        [Test]
        public void ValidConfig() {
            var def = Loggy.Get();
            Assert.NotNull(def);
            Assert.True(def.Isolated);
            Assert.AreEqual(1,def.Appenders.Count);
            Assert.AreEqual(appender,def.Appenders[0]);
            Assert.AreEqual("default",def.Name);
        }



        [SetUp]
        public void Setup()
        {
            Loggy.Manager = null;
            Loggy.Manager = new LoggyManager();
            this.appender = new TestAppender{Level = LogLevel.Info};
            Loggy.Get().Appenders.Add(appender);
            Loggy.Level = LogLevel.Trace;
        }

    }
}
