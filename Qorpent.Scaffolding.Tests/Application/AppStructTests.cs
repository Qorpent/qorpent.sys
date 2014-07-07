using NUnit.Framework;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
    public class AppStructTests : AppModelBasicTestsBase {

        [Test]
        public void CanReadStructFields() {
            var model = TestModel(
                @"namespace test
    class Struct abstract prototype=ui-data
    Struct S1 ""Пример структуры""
        bool isok ""Все ли хорошо"" : true
"
                );
        }
    }
}