using System.IO;
using NUnit.Framework;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests.ConsoleApplicationSupport {
    [TestFixture]
    public class ConsoleParametersLogRedirectionTest {
        [Test]
        public void CanRedirectUserLogToWriter() {
            var p = new ConsoleApplicationParameters {LogFormat = "${Message}"};
            p.Initialize();
            var sw = new StringWriter();
            p.RedirectLog(sw);
            p.Log.Info("test");
            p.WaitLog();
            p.ResetRedirectLog();
            Assert.AreEqual("test".Simplify(SimplifyOptions.Full),sw.ToString().Simplify(SimplifyOptions.Full));
        }

        [Test]
        public void CanRedirectUserLogToWriterGlobalizedToLoggy() {
            var p = new ConsoleApplicationParameters { LogFormat = "${Message}" };
            p.Initialize();
            var sw = new StringWriter();
            p.RedirectLog(sw);
            Loggy.Default.Info("test");
            p.WaitLog();
            p.ResetRedirectLog();
            Assert.AreEqual("test".Simplify(SimplifyOptions.Full), sw.ToString().Simplify(SimplifyOptions.Full));
        }
        [Test]
        public void CanRedirectUserLogToWriterGlobalizedToLoggyOverride()
        {
            var p = new ConsoleApplicationParameters { LogFormat = "${Message}" };
            p.Initialize();
            var sw = new StringWriter();
            p.RedirectLog(sw);
            Loggy.Manager = new LoggyManager();
            Loggy.Default.Info("test");
            p.WaitLog();
            p.ResetRedirectLog();
            Assert.AreEqual("test".Simplify(SimplifyOptions.Full), sw.ToString().Simplify(SimplifyOptions.Full));
        }
    }
}