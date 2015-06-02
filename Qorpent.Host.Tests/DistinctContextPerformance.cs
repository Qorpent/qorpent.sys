using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.IO.Net;
using Qorpent.Mvc;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Tests
{
    [TestFixture(Description = "Проверка производительности QH в различных конфигурациях")]
    [Explicit]
    public class DistinctContextPerformance
    {
        private HostServer host;
        private HttpClient cli;

        public interface IApi {
            bool Execute(int wait = 10);
        }
        public class Api:IApi
        {
            public bool Execute(int wait = 10)
            {
                Thread.Sleep(wait);
                return true;
            }
        }
        
        public class ApiOptions {
            public int Wait { get; set; }
        }

        
        [Action("api.execute")]
        public class ApiExecuteAction : ActionBase {
            [Bind]
            public ApiOptions Options { get; set; }
            public Api Api = new Api();
            protected override object MainProcess() {
                return Api.Execute(Options.Wait);
            }
        }

        public class ApiRequestHandler : IRequestHandler {
            public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
                var wait = context.Uri.Query.Split('=')[1].ToInt();
                var api = server.Container.Get<IApi>();
                try {
                    context.Finish(api.Execute(wait).ToStr().ToLowerInvariant());
                }
                finally {
                    server.Container.Release(api);
                }
            }
        }

        [SetUp]
        public void Setup() {
            host = new HostServer(14910);
            host.Initialize();
            host.Container.Get<IMvcFactory>().Register(typeof (ApiExecuteAction));
            host.Container.Register(host.Container.NewComponent<IApi,Api>(Lifestyle.Pooled));
            host.Factory.Register("/apir/execute",new ApiRequestHandler());
            host.Start();
            Thread.Sleep(100);
            cli = new HttpClient();
        }

        [TearDown]
        public void TearDown() {
            host.Stop();
        }

        [Test]
        public void CanCallWithAction() {
            var result = GetResult();
            Assert.AreEqual("true",result);
        }

        [Test]
        public void CanCallWithRequest()
        {
            var result = GetResult("r");
            Assert.AreEqual("true", result);
        }

        [TestCase("", 0, 10000)]
        [TestCase("r", 0, 10000)]
        [TestCase("", 1, 1000)]
        [TestCase("r", 1, 1000)]
        [TestCase("", 10, 100)]
        [TestCase("r", 10, 100)]
        public void SequenceTimeStamp(string api,int wait,int count) {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++) {
                Assert.AreEqual("true",GetResult(api,wait));
            }
            sw.Stop();
            Console.WriteLine("API"+api+" " +sw.Elapsed);
        }

        [TestCase("", 0, 10000)]
        [TestCase("r", 0, 10000)]
        [TestCase("", 1, 1000)]
        [TestCase("r", 1, 1000)]
        [TestCase("", 10, 100)]
        [TestCase("r", 10, 100)]
        public void ParallelTimeStampSingle(string api,int wait,int count) {
            
            var tasks = new List<int>();
            for (var i = 0; i < count; i++) {
                tasks.Add(i);
            }
            var sw = Stopwatch.StartNew();
            tasks.AsParallel().ForAll(_ => {
                Assert.AreEqual("true",GetResult(api,wait));
            });
            sw.Stop();
            Console.WriteLine("API" + api + " " + sw.Elapsed);
        }

        private string GetResult(string type = "", int wait=10) {
            var result = cli.GetString(string.Format("http://127.0.0.1:14910/api{0}/execute?wait={1}",  type, wait));
            return result;
        }
    }
}
