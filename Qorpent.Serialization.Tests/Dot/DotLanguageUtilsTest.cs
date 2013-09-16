using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Graphs.Dot;

namespace Qorpent.Serialization.Tests.Dot
{
    [TestFixture]
    public class DotLanguageUtilsTest
    {
        [TestCase("aРус", "aРус")]
        [TestCase("a1", "a1")]
        [TestCase("_a1", "_a1")]
        [TestCase("1a", "_0x0031a")]
        [TestCase("a,b", "a_0x002Cb")]
        [TestCase("a,b>c", "a_0x002Cb_0x003Ec")]
        public void CodeEscaping(string code, string result) {
            Assert.AreEqual(result,DotLanguageUtils.EscapeCode(code));
        }

        [TestCase("aРус", "aРус")]
        [TestCase("a1", "a1")]
        [TestCase("_a1", "_a1")]
        [TestCase("1a", "_0x0031a")]
        [TestCase("a,b", "a_0x002Cb")]
        [TestCase("a,b>c", "a_0x002Cb_0x003Ec")]
        public void CodeUnEscaping(string result, string code)
        {
            Assert.AreEqual(result, DotLanguageUtils.UnEscapeCode(code));
        }
        [Test]
        public void UnderlineCanStartLiteral() {
            Assert.True(DotLanguageUtils.IsLiteral("_a"));
            Assert.True(DotLanguageUtils.IsLiteral("_a__1b"));
        }

        [TestCase("int",1,"1")]
        [TestCase("bool",true,"true")]
        [TestCase("bool2",false,"false")]
        [TestCase("double",0.23,"0.23")]
        [TestCase("literal","_ab1_c","_ab1_c")]
        [TestCase("nliteral","1_ab1_c","\"1_ab1_c\"")]
        [TestCase("unicode", "рус", "\"&#1088;&#1091;&#1089;\"")]
        [TestCase("non_u_r_l", "http://x/?a=v v", "\"http://x/?a=v v\"")]
        [TestCase("withURL", "http://x/?a=v v", "\"http://x/?a=v%20v\"")]
        [TestCase("withURLandplus", "http://x/?a=v+v", "\"http://x/?a=v%2Bv\"")]
        [TestCase("withURLandplusAndUnicode", "http://x/?a=v+v&name=рус", "\"http://x/?a=v%2Bv&amp;name=&#1088;&#1091;&#1089;\"")]
        public void AttributeValueTest(string attr, object val, string result) {
            Assert.AreEqual(result,DotLanguageUtils.GetAttributeString(attr,val));
        }
    }
}
