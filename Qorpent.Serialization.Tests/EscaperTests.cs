using NUnit.Framework;
using Qorpent.Serialization.Escaping;

namespace Qorpent.Serialization.Tests
{
    [TestFixture]
    public class EscaperTests
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
        public void TestXmlNameEscaping(string input, string output)
        {
            Assert.AreEqual(output, input.Escape(EscapingType.XmlName));
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
        public void TestXmlNameUnEscaping(string output, string input)
        {
            Assert.AreEqual(output, input.Unescape(EscapingType.XmlName));
        }

        [TestCase("quo\"te", "quo&quot;te")]
        [TestCase("&amp&rsand&", "&amp;amp&amp;rsand&amp;")]
        [TestCase("√unicodesymbols;∞", "&#x221A;unicodesymbols;&#x221E;")]
        public void TestXmlAttributeEscaping(string input, string output)
        {
            Assert.AreEqual(output, input.Escape(EscapingType.XmlAttribute));
        }

        [TestCase("quo\"te", "quo&quot;te")]
        [TestCase("&amp&rsand&", "&amp;amp&amp;rsand&amp;")]
        [TestCase("√unicodesymbols;∞", "&#x221A;unicodesymbols;&#x221E;")]
        public void TestXmlAttributeUnescaping(string output, string input)
        {
            Assert.AreEqual(output, input.Unescape(EscapingType.XmlAttribute));
        }

        [TestCase("quo\"te", "quo__QUOT__te")]
        [TestCase("=equals=", "__EQ__equals__EQ__")]
        [TestCase("√unicodesymbols;∞", "√unicodesymbols;∞")]
        public void TestBxlLiteralEscaping(string input, string output)
        {
            Assert.AreEqual(output, input.Escape(EscapingType.BxlLiteral));
        }

        [TestCase("quo\"te", "quo__QUOT__te")]
        [TestCase("=equals=", "__EQ__equals__EQ__")]
        [TestCase("√unicodesymbols;∞", "√unicodesymbols;∞")]
        public void TestBxlLiteralUnescaping(string output, string input)
        {
            Assert.AreEqual(output, input.Unescape(EscapingType.BxlLiteral));
        }

        [TestCase('q', EscapingType.XmlName, true, true)]
        [TestCase('.', EscapingType.XmlName, false, true)]
        [TestCase('.', EscapingType.XmlName, true, false)]
        [TestCase('1', EscapingType.XmlName, false, true)]
        [TestCase('1', EscapingType.XmlName, true, false)]
        [TestCase('&', EscapingType.XmlName, false, false)]
        [TestCase('&', EscapingType.XmlName, true, false)]
        [TestCase('q', EscapingType.XmlAttribute, true, true)]
        [TestCase('.', EscapingType.XmlAttribute, true, true)]
        [TestCase('1', EscapingType.XmlAttribute, true, true)]
        [TestCase('&', EscapingType.XmlAttribute, true, false)]
        [TestCase('q', EscapingType.BxlLiteral, true, true)]
        [TestCase('.', EscapingType.BxlLiteral, true, true)]
        [TestCase('1', EscapingType.BxlLiteral, true, true)]
        [TestCase(':', EscapingType.BxlLiteral, true, false)]
        public void TestCheckingLiteral(char c, EscapingType type, bool first, bool res)
        {
            Assert.AreEqual(Escaper.IsLiteral(c, type, first), res);
        }

        [TestCase("qwerty", EscapingType.XmlName, true)]
        [TestCase(".qwerty", EscapingType.XmlName, false)]
        [TestCase("qwe&rty", EscapingType.XmlName, false)]
        [TestCase("qwerty", EscapingType.XmlAttribute, true)]
        [TestCase("qw&erty", EscapingType.XmlAttribute, false)]
        [TestCase("qwerty", EscapingType.BxlLiteral, true)]
        [TestCase("qwe:rty", EscapingType.BxlLiteral, false)]
        public void TestCheckingLiterals(string c, EscapingType type, bool res)
        {
            Assert.AreEqual(Escaper.IsLiteral(c, type), res);
        }
    }
}
