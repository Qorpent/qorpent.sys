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

    }
}
