using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
    [TestFixture]
    public class DictionarySupport : CompileTestBase {
        [Test]
        public void ExportKeywordRemoved() {
            var code = @"
class A
    export x
";
            var compiled = Compile(code).Get("A").Compiled;
            Assert.False(compiled.Elements("export").Any());
        }

        [Test]
        public void CanSupplyValue()
        {
            var code = @"
class A
    export x
    item a : test

class B y=??x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("test",compiled.Attr("y"));
        }
        [Test]
        public void CanSupplyNotResolvedValue()
        {
            var code = @"
class B y=??x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("notresolved:x.a", compiled.Attr("y"));
        }
        [Test]
        public void CanAvoidNotResolvedValue()
        {
            var code = @"
class B y=??~x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("", compiled.Attr("y"));
        }
        [Test]
        public void CanAvoidNotResolvedRef()
        {
            var code = @"
class B y=?~x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("", compiled.Attr("y"));
        }
        [Test]
        public void CanSupplyNotResolvedRef()
        {
            var code = @"
class B y=?x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("notresolved:x.a", compiled.Attr("y"));
        }

        [Test]
        public void CanSupplyValueAndRef()
        {
            var code = @"
namespace X
    class A
        export x
        item a : test
class B y=?x.a
";
            var compiled = Compile(code).Get("B").Compiled;
            Assert.AreEqual("test|X.A:item:a", compiled.Attr("y"));
        }
    }
}