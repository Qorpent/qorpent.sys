using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Host;

namespace Qorpent.Core.Tests.Log {
    [TestFixture]
    public class UdpTraceListenerTests {
        [Test]
        public void TestXml() {
            var u = new UdpTraceListener();
            //var r = u.GetEventXml("test message", "debug");
            var r = u.GetEventXml("test message", "error");
            //Trace.TraceError(new Exception("fuc"));
            Console.WriteLine(r);
        }


    }
}
