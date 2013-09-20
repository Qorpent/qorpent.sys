using System;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests
{
    [TestFixture]
    //[Explicit]
    //[Ignore("Dmitry must fix!!!")]
    //TODO: DMIT - must fix
    public class XmlNameEscaperTest
    {
        [TestCase("in-minus", "in-minus")]
        [TestCase("in1dec", "in1dec")]
        [TestCase("1leaddec", "_1leaddec")]
        [TestCase("-leaddefis", "__MINUS__leaddefis")]
        [TestCase("in.dot", "in.dot")]
        [TestCase(".leaddot", "__DOT__leaddot")]
        [TestCase("русское", "русское")]
        [TestCase("!a*", "__EXC__a__STAR__")]
        [TestCase("√unicode__symbols∞", "__0x221A__unicode__symbols__0x221E__")]
        public void TestEscaping(string input, string output) {
            Assert.AreEqual(output,input.Escape(EscapingType.XmlName));
        }

        [TestCase("in-defis", "in-defis")]
        [TestCase("in1dec", "in1dec")]
        [TestCase("1leaddec", "_1leaddec")]
        [TestCase("-leaddefis", "__MINUS__leaddefis")]
        [TestCase("in.dot", "in.dot")]
        [TestCase(".leaddot", "__DOT__leaddot")]
        [TestCase("русское", "русское")]
        [TestCase("!a*", "__EXC__a__STAR__")]
        [TestCase("√unicode__symbols∞", "__0x221A__unicode__symbols__0x221E__")]
        [TestCase("√unicode__symbol∞", "__0x221A__unicode__symbol__0x221E__")]
        public void TestUnEscaping(string output, string input)
        {
            Assert.AreEqual(output, input.Unescape(EscapingType.XmlName));
        }
    }
}
