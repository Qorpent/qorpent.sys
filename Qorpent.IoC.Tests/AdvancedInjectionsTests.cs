using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Qorpent.IoC.Tests {
    [TestFixture]
    public class AdvancedInjectionsTests {
        interface IX {
            int Id { get; }
        }
        class X:IX {
            public int Id { get; set; }
        }

        interface IY {
            IList<IX> SmallIds { get; }
            IList<IX> LargeIds { get; }
            int LargeSum { get; set; }
            int SmallSum { get; set; }
        }

        class Y : IY,IContainerBound {
            public Y() {
                LargeIds = new List<IX>();
                SmallIds = new List<IX>();
            }
            [Inject(NameMask = "x")]
            public IList<IX> SmallIds { get; private set; }

            [Inject(NameMask = "y")]
            public IList<IX> LargeIds { get; private set; }

            public void SetContainerContext(IContainer container, IComponentDefinition component) {
                this.Container = container;
            }

            public IContainer Container { get; set; }

            public void OnContainerRelease() {
            }

            public void OnContainerCreateInstanceFinished() {
                this.SmallSum = SmallIds.Sum(_ => _.Id);
                this.LargeSum = LargeIds.Sum(_ => _.Id);
            }

            public int LargeSum { get; set; }

            public int SmallSum { get; set; }
        }

        class YF : IFactory {
            public object Get(Type serviceType, string name = "") {
                return new Y();
            }
        }
        interface IZ
        {
            IY Y { get; set; } 
            IY Y2 { get; set; } 
        }

        class Z:IZ {
            [Inject(DefaultType = typeof(Y))]
            public IY Y { get; set; }
            [Inject(FactoryType = typeof(YF))]
            public IY Y2 { get; set; }
        }
        [Test]
        public void Q320NameMaskSupport() {
            var c = SetupContainer();
            c.Register(c.NewComponent<IY, Y>());
            var y = c.Get<IY>();
            Assert.AreEqual(6,y.SmallIds.Sum(_=>_.Id));
            Assert.AreEqual(60,y.LargeIds.Sum(_=>_.Id));
            Assert.AreEqual(6, y.SmallSum);
            Assert.AreEqual(60, y.LargeSum);
        }
        [Test]
        public void Q321ConainerAllSupportsRegex()
        {
            var c = SetupContainer();
            var x = c.All<IX>();
            Assert.AreEqual(6,x.Count());
             x = c.All<IX>(name:"~x");
            Assert.AreEqual(3, x.Count());
            Assert.True(x.All(_=>_.Id<10));
             x = c.All<IX>(name:"~y");
            Assert.AreEqual(3, x.Count());

            Assert.True(x.All(_ => _.Id >= 10));
            x = c.All<IX>(name: "y");
            Assert.AreEqual(0, x.Count());
            x = c.All<IX>(name: "a.y1");
            Assert.AreEqual(1, x.Count());
        }
        [Test]
        public void Q319DefaultTypeSupport()
        {
            var c = SetupContainer();
            c.Register(c.NewComponent<IZ, Z>());
            var z = c.Get<IZ>();
            Assert.IsInstanceOf<Y>(z.Y);
        }
        [Test]
        public void Q322DefaultTypeMakeInjects()
        {
            var c = SetupContainer();
            c.Register(c.NewComponent<IZ, Z>());
            var z = c.Get<IZ>();
            Assert.AreEqual(6, z.Y.SmallIds.Sum(_ => _.Id));
            Assert.AreEqual(60, z.Y.LargeIds.Sum(_ => _.Id));
            Assert.AreEqual(6, z.Y.SmallSum);
            Assert.AreEqual(60, z.Y.LargeSum);
        }
        [Test]
        public void Q322FactoryMakeInjects()
        {
            var c = SetupContainer();
            c.Register(c.NewComponent<IZ, Z>());
            var z = c.Get<IZ>();
            Assert.AreEqual(6, z.Y2.SmallIds.Sum(_ => _.Id));
            Assert.AreEqual(60, z.Y2.LargeIds.Sum(_ => _.Id));
            Assert.AreEqual(6, z.Y2.SmallSum);
            Assert.AreEqual(60, z.Y2.LargeSum);
        }
        [Test]
        public void Q319DefaultTypeNotOverrideDefaultBehavior()
        {
            var c = SetupContainer();
            var y = new Y();
            c.Register(c.NewComponent<IZ, Z>());
            c.Register(c.NewComponent<IY, Y>(implementation:y));
            var z = c.Get<IZ>();
            Assert.AreSame(y,z.Y);
        }

        private static Container SetupContainer() {
            var c = new Container();
            c.Register(c.NewComponent<IX, X>(name: "a.x1", implementation: new X {Id = 1}));
            c.Register(c.NewComponent<IX, X>(name: "a.x2", implementation: new X {Id = 2}));
            c.Register(c.NewComponent<IX, X>(name: "a.x3", implementation: new X {Id = 3}));
            c.Register(c.NewComponent<IX, X>(name: "a.y1", implementation: new X {Id = 10}));
            c.Register(c.NewComponent<IX, X>(name: "a.y2", implementation: new X {Id = 20}));
            c.Register(c.NewComponent<IX, X>(name: "a.y3", implementation: new X {Id = 30}));
            
            return c;
        }
    }
}