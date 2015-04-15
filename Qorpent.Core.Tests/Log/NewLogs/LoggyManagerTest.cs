using NUnit.Framework;

namespace Qorpent.Core.Tests.Log.NewLogs {
    [TestFixture]
    public class LoggyManagerTest {
        private LoggyManager manager;

        [SetUp]
        public void Setup() {
            this.manager = new LoggyManager();
        }

        [Test]
        public void CanGetDefaultLogger() {
            var l = manager.Get();
            var l2 = manager.Get("default");
            var l3 = manager.Get();
            var l4 = manager.Get("default");
            Assert.True((l==l2)&&(l2==l3)&&(l3==l4));
        }

        [Test]
        public void CanGetExplicitlySetNamed() {
            var l = new DefaultLoggy();
            manager.Loggers["x"] = l;
            Assert.AreEqual(l,manager.Get("x"));

        }


        [Test]
        public void CanGetNamed() {
            var l = manager.Get("x");
            Assert.NotNull(l);
            var l2 = manager.Get("x");
            Assert.AreEqual(l,l2);
            var d = manager.Get();
            Assert.True(l.SubLoggers.Contains(d));
            Assert.True(l.SubLoggers.Count==1);

        }

    }
}