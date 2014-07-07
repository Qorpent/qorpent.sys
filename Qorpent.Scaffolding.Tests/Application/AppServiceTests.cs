using NUnit.Framework;
using Qorpent.Scaffolding.Application;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
    public class AppServiceTests : AppModelBasicTestsBase {
        [Test]
        public void CanParceSubscriptions() {
            var model = TestModel(@"
class service s prototype=ui-service
    subscribe PUBLISHED
");
            var service = model.Resolve<AppService>("service");
            Assert.IsNotNull(service);
            Assert.IsNotNull(service.Subscriptions);
            Assert.AreEqual(1, service.Subscriptions.Count);
        }
    }
}