using NUnit.Framework;

namespace Qorpent.IoC {
    class ZC_538Tests : ZC_538TestsBase {

        [Test]
        public void CanGetAnExtensionClassFromContainer() {
            var extCalss = Container.Get<IExtensionClass>();
            Assert.IsNotNull(extCalss);
            Assert.IsInstanceOf<IExtensionClass>(extCalss);
        }

        [Test]
        public void CanGetAHostClassFromContainer() {
            var hostClass = Container.Get<IHostClass>();
            Assert.IsNotNull(hostClass);
            Assert.IsInstanceOf<IHostClass>(hostClass);
        }

        [Test]
        public void CanInjectExtClassesToTheHostClassIfInject() {
            var hostClass = Container.Get<IHostClass>();
            Assert.IsNotNull(hostClass);
            Assert.IsInstanceOf<IHostClass>(hostClass);
            Assert.AreEqual(3, hostClass.List.Count);
        }
    }
}
