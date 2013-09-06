﻿using System.IO;
using System.Text;
using NUnit.Framework;

namespace Qorpent.IO.Tests {
    [TestFixture]
    public class StreamProxyTests {
        [Test]
        public void CanUseProxy() {
            var proxy = new StreamProxy();

            var source = new MemoryStream();
            var target1 = new MemoryStream();
            var target2 = new MemoryStream();

            source.Write(Encoding.UTF8.GetBytes("test"), 0, 4);
            source.Position = 0;

            proxy.Proxy(source, target1, target2);
            var target1Got = new StreamReader(target1).ReadToEnd();
            var target2Got = new StreamReader(target2).ReadToEnd();

            Assert.NotNull(target1Got);
            Assert.NotNull(target2Got);

            Assert.AreEqual(target1Got, target2Got);
            Assert.AreEqual("test", target1Got);
        }
    }
}
