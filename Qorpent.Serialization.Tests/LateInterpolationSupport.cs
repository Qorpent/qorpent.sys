using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Serialization.Tests.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests
{
    [TestFixture]
    public class LateInterpolation:CompileTestBase
    {
        [Test]
        public void CanSetupInterpolationAtLastWithDictionary() {
            var context = Compile(@"
class A
    export a
    item b : 1
class B y=??a.b x='-#{y}-'
");
            var cls = context.Get("B").Compiled;
            Assert.AreEqual("-1-",cls.Attr("x"));
        }

        
    }

    [TestFixture]
    public class ElementRewriteFeature:CompileTestBase {
        [Test]
        public void RewriteElementSupportSimple() {
            var context = Compile(@"
class A
    element test rewrite 
        test2 %{element.code} z=%{element.x}
    test u x=1
        a
");
            var cls = context.Get("A").Compiled;
            var str = cls.ToString().Simplify(SimplifyOptions.Test);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' fullcode='A'>
  <test2 code='u' z='1' />
</class>".Simplify(SimplifyOptions.Test), str);

        }

        [Test]
        public void RewriteElementSupportSimple_WithName()
        {
            var context = Compile(@"
class A
    element test rewrite 
        test2 %{element.@name} t=%{element.name}
    test u a
");
            var cls = context.Get("A").Compiled;
            var str = cls.ToString().Simplify(SimplifyOptions.Test);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' fullcode='A'>
  <test2 code='a' t='test' />
</class>".Simplify(SimplifyOptions.Test), str);

        }

        [Test]
        public void RewriteElementSupportCopyAttributes()
        {
            var context = Compile(@"
class A
    element test rewrite copy
        test2 %{element.code} z=%{element.x}
    test u x=1
        a
");
            var cls = context.Get("A").Compiled;
            var str = cls.ToString().Simplify(SimplifyOptions.Test);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' fullcode='A'>
  <test2 code='u' z='1' x='1'>
    <a />
  </test2>
</class>".Simplify(SimplifyOptions.Test), str);

        }

        [Test]
        public void KeepInterpolation() {
            var context = Compile(@"
class A f=2
    element test rewrite 
        test2 %{element.code} z=%{element.x} m=${f}
    test u x=1
        
");
            var cls = context.Get("A").Compiled;
            var str = cls.ToString().Simplify(SimplifyOptions.Test);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' f='2' fullcode='A'>
  <test2 code='u' z='1' m='2' />
</class>".Simplify(SimplifyOptions.Test), str);
        }

        [Test]
        public void CompatibleWithIncludes() {
            var context = Compile(@"
class B a=1
class A
    element test rewrite 
        include %{element.code}
    test B
");
            var cls = context.Get("A").Compiled;
            var str = cls.ToString().Simplify(SimplifyOptions.Test);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' fullcode='A'>
  <B a='1' />
</class>".Simplify(SimplifyOptions.Test), str);
        }
    }
}
