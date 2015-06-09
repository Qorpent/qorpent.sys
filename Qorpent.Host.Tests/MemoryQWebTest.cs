using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Mvc;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Tests
{
    [Action("test.best",Role = "GUEST")]
    class TestAction : ActionBase {
        private static int i = 0;
        public static int creates = 0;
        public TestAction() {
            creates++;
        }
        protected override object MainProcess()
        {
            Thread.Sleep(333);
            return i++;
        }
    }

    [TestFixture]
    public class MemoryQWebTest
    {
        

        [Test]
        public void CanRunSingle() {
            var hs = new HostServer(14910);
            hs.Initialize();
            var mvc= (MvcFactory)hs.Container.Get<IMvcFactory>();
            mvc.Register(typeof (TestAction).Assembly);
            var result = hs.Call("/test/best");
            Console.WriteLine(result);
            Assert.AreEqual("0",result);
            Assert.AreEqual(1,mvc.GetMetric("action.pool.count"));
            result = hs.Call("/test/best");
            Assert.AreEqual("1", result);
            Assert.AreEqual(1, TestAction.creates);
            Assert.AreEqual(1, mvc.GetMetric("action.pool.count"));
            
        }

        [Test]
        public void HighLoad() {
            var hs = new HostServer(14910);
            hs.Initialize();
            var mvc = (MvcFactory)hs.Container.Get<IMvcFactory>();
            mvc.Register(typeof(TestAction).Assembly);
            var tasks = new List<Task>();
            for (var i = 0; i < 20; i++) {
                for (var j = 0; j < 4; j++) {
                    tasks.Add(Task.Run(() => {
                        hs.Call("/test/best");
                    }));
                }
                Console.WriteLine(i);
                Thread.Sleep(200);
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine(mvc.GetMetric("action.pool.count"));
            Console.WriteLine(TestAction.creates);
            Assert.Greater(10,mvc.GetMetric("action.pool.count").ToInt());
            Assert.Greater(10,TestAction.creates);
        }


    }
}
