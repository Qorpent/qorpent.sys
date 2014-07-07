using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
    public class AppActionTests : AppModelBasicTestsBase {
        [Test]
        public void CanReadActions() {
            TestModel(
                @"
class A2 name2 prototype=ui-action
class A1 name1 prototype=ui-action
",
                @"
AppAction A1 'name1'
AppAction A2 'name2'
"
                );
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
