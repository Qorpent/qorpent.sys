using System;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.data.agg;
using Qorpent.Experiments;
using Qorpent.Model;

namespace Qorpent.Serialization.Tests.v2.agg
{
    [TestFixture]
    public class AggregationTest
    {
        class Obj {
            public string Key1 { get; set; }
            public string Key2 { get; set; }
            public int Sum { get; set; }
        }

        Obj[] objs = {
                new Obj {Key1 = "a", Key2 = "x", Sum = 1},
                new Obj {Key1 = "a", Key2 = "y", Sum = 2},
                new Obj {Key1 = "b", Key2 = "x", Sum = 3},
                new Obj {Key1 = "b", Key2 = "y", Sum = 4},
            };

        [Test]
        public void NestAggregation() {
            var agg = new Aggregator();
            var ret1 = new Router {Key="k1",Retriever = (o,s)=>new RouteKey((o as Obj).Key1)};
            var ret2= new Router {Parent=ret1,Key="k2",Retriever = (o,s)=>new RouteKey((o as Obj).Key2)};
            ret1.Children.Add(ret2);
            agg.Register(ret1);
            agg.Register(new SumCollector {Key="s", ValueFunction = (o, c, node, s) => (o as Obj).Sum });
            foreach (var obj in objs)
            {
                agg.Add(obj);
            }
            var n = agg.GetResult();
            Assert.AreEqual(10, n.GetValue("s"));
            Assert.AreEqual(10, n.GetValue("k1", "s"));
            Assert.AreEqual(3, n.GetValue("k1", "a","s"));
            Assert.AreEqual(7, n.GetValue("k1", "b","s"));
            Assert.AreEqual(3, n.GetValue("k1", "a", "k2", "s"));
            Assert.AreEqual(1, n.GetValue("k1", "a", "k2", "x","s"));
            Assert.AreEqual(4, n.GetValue("k1", "b", "k2", "y","s"));
        }

        [Test]
        public void FilteredNest()
        {
            var agg = new Aggregator();
            var ret1 = new Router { Key = "k1", Retriever = (o, s) => new RouteKey((o as Obj).Key1) };
            var ret2 = new Router { Parent = ret1, Key = "k2",
                Retriever = (o, s) => new RouteKey((o as Obj).Key2) ,
            FilterFunc = (o,node,scope)=> (o as Obj).Sum>1 };
            ret1.Children.Add(ret2);
            agg.Register(ret1);
            agg.Register(new SumCollector { Key = "s", ValueFunction = (o, c, node, s) => (o as Obj).Sum });
            foreach (var obj in objs)
            {
                agg.Add(obj);
            }
            var n = agg.GetResult();
            Assert.AreEqual(10, n.GetValue("s"));
            Assert.AreEqual(10, n.GetValue("k1", "s"));
            Assert.AreEqual(3, n.GetValue("k1", "a", "s"));
            Assert.AreEqual(7, n.GetValue("k1", "b", "s"));
            Assert.AreEqual(2, n.GetValue("k1", "a", "k2", "s"));
            Assert.AreEqual(null, n.GetValue("k1", "a", "k2", "x", "s"));
            Assert.AreEqual(4, n.GetValue("k1", "b", "k2", "y", "s"));
        }

        [Test]
        public void SimplestAggregation() {
            
            var agg = new Aggregator();
            agg.Register((o, s) => new RouteKey((o as Obj).Key1));
            agg.Register((o, s) => new RouteKey((o as Obj).Key2));
            agg.Register(new SumCollector {ValueFunction = (o,c,node,s)=>(o as Obj).Sum});
            foreach (var obj in objs) {
                agg.Add(obj);
            }
            var n = agg.GetResult();
            Assert.AreEqual(10,n.GetValue("val0"));
            Assert.AreEqual(10,n.GetValue("agg0","val0"));
            Assert.AreEqual(10,n.GetValue("agg1","val0"));
            Assert.AreEqual(3,n.GetValue("agg0","a","val0"));
            Assert.AreEqual(7,n.GetValue("agg0","b","val0"));
            Assert.AreEqual(4,n.GetValue("agg1","x","val0"));
            Assert.AreEqual(6,n.GetValue("agg1","y","val0"));

            Assert.AreEqual(10, n.GetValue("/val0"));
            Assert.AreEqual(10, n.GetValue("/agg0/val0"));
            Assert.AreEqual(10, n.GetValue("/agg1/val0"));
            Assert.AreEqual(3, n.GetValue("/agg0/a/val0"));
            Assert.AreEqual(7, n.GetValue("/agg0/b/val0"));
            Assert.AreEqual(4, n.GetValue("/agg1/x/val0"));
            Assert.AreEqual(6, n.GetValue("/agg1/y/val0"));
            
            Assert.AreEqual(10, n.GetValue("agg0.val0"));
            Assert.AreEqual(10, n.GetValue("agg1.val0"));
            Assert.AreEqual(3, n.GetValue("agg0.a.val0"));
            Assert.AreEqual(7, n.GetValue("agg0.b.val0"));
            Assert.AreEqual(4, n.GetValue("agg1.x.val0"));
            Assert.AreEqual(6, n.GetValue("agg1.y.val0"));

            var j = n.stringify();
            Console.WriteLine(j);
            
        }
    }
}
