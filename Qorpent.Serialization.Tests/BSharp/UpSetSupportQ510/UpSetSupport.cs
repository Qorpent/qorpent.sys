using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp.UpSetSupportQ510
{
    [TestFixture]
    public class UpSetSupport: CompileTestBase
    {
        [Test]
        public void SimpleAtClassLevel() {
            var code = @"
class a
    up-set b=1
";
            var cls = Compile(code).Get("a");
            var xml = cls.Compiled.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Assert.AreEqual(@"<class code='a' fullcode='a' b='1' />", xml);
        }

        [Test]
        public void ConditionalAtClassLevel()
        {
            var code = @"
class a x=1 z=a${b}${c}a
    up-set b=1 if=x
    up-set b=2 if=y
    up-set c=3 if=x

";
            var cls = Compile(code).Get("a");
            var xml = cls.Compiled.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Assert.AreEqual(@"<class code='a' x='1' z='a13a' fullcode='a' b='1' c='3' />", xml);
        }

        [Test]
        public void UpSetOnNestLevel() {
            var code = @"
class a x=1
    e
        up-set b=1 if=x
        up-set b=2 if=y
        up-set c=3 if=x

";
            var cls = Compile(code).Get("a");
            var xml = cls.Compiled.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(xml);
            Assert.AreEqual(@"<class code='a' x='1' fullcode='a'>
  <e b='1' c='3' />
</class>", xml);
        }
    }
}
