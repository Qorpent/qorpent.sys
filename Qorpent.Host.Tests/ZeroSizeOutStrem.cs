using System;
using System.Threading;
using NUnit.Framework;
using Qorpent.IO.Http;
using Qorpent.IO.Net;

namespace Qorpent.Host.Tests
{
    [TestFixture]
    public class ZeroSizeOutStrem
    {
        class zero_data : IRequestHandler {
            public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
                context.Finish("");
            }
        }
        class no_zero_data : IRequestHandler
        {
            public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel)
            {
                context.Finish("It's not zero","text/plain");
            }
        }
        [Test]
        public void ZeroDataHttpClientFail_BUG_Q540() {
            var h = new HostServer(8091);
            h.Factory.Register("/test",new zero_data());
            try {
                h.Start();
                Assert.AreEqual("",new HttpClient().GetString("http://127.0.0.1:8091/test"));
            }
            finally {
                h.Stop();
            }
        }

        [Test]
        public void Head_Method_Support_BUG_Q541()
        {
            var h = new HostServer(8091);
            h.Factory.Register("/test", new no_zero_data());
            try
            {
                h.Start();
                var req = new HttpRequest
                {
                    Uri = new Uri("http://127.0.0.1:8091/test"),
                    Method = "GET"
                };
                Assert.AreEqual("It's not zero", new HttpClient().Call(req).StringData);
                req.Method = "HEAD";
                Assert.AreEqual("", new HttpClient().Call(req).StringData);
            }
            finally
            {
                h.Stop();
            }
        }
    }
}
