using NUnit.Framework;
using Qorpent.Scaffolding.Application;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
    public class AppActionWriterTests : AppModelBasicTestsBase {
        [Test]
        public void CanReadActions() {
            var model =TestModel(
                @"
class A2 name2 prototype=ui-action
class A1 name1 prototype=ui-action
"
                );
            var action = model.Actions["A1"];
            var generator = new AppActionWriter().Setup(action);
            Assert.AreEqual(@"
zzzzzzzzzzzzzzzzzzzz
",generator.ToString());
        }


        [Test]
        public void CanReadActionParameters() {
            var model = TestModel(
                @"
namespace test
    class S1 s1 prototype=ui-data
    class S2 s2 prototype=ui-data
    class A1 name1 prototype=ui-action
        Url = /a1
        Arguments =^S1
        Result = ^S2*

"
                );
            var action = model.Actions["A1"];

            Assert.AreEqual("A1", action.Code);
            Assert.AreEqual("name1", action.Name);
            Assert.AreEqual("/a1", action.Url);
            Assert.AreEqual("test.S1", action.ArgumentsReference);
            Assert.AreEqual("test.S2", action.ResultReference);
            Assert.True(action.ResultIsArray);

            var arguments = model.Structs["S1"];
            Assert.AreEqual(arguments, action.Arguments);
            var result = model.Structs["S2"];
            Assert.AreEqual(result, action.Result);

        }
    }
}