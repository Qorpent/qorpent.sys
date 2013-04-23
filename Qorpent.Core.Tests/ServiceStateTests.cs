using NUnit.Framework;
using Qorpent.Mvc.HttpHandler;

namespace Qorpent.Core.Tests {
    internal class ServiceEmulation : ServiceBase {
        
    }


    class ServiceStateTests {
        private ServiceEmulation _service;
        private MvcHandler _mvcHandler;

        [SetUp]
        public void TestsSetUp() {
            _service = new ServiceEmulation();
        }

        [Test]
        public void CanCorrectRegisterHandlerInStatistics() {
            Assert.AreEqual(0, ServiceState.CurrentHandlers);
            _mvcHandler = new MvcHandler();
            Assert.AreNotEqual(0, ServiceState.CurrentHandlers);
            _mvcHandler.Dispose();
            Assert.AreEqual(0, ServiceState.CurrentHandlers);
        }
    }
}
