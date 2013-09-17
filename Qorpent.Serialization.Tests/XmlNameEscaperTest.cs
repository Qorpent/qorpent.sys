using NUnit.Framework;

namespace Qorpent.Serialization.Tests
{
    [TestFixture]
    [Explicit]
    [Ignore("Dmitry must fix!!!")]
    //TODO: DMIT - must fix
    public class XmlNameEscaperTest
    {
        [TestCase("in-minus", "in-minus")]
        [TestCase("in1dec", "in1dec")]
        [TestCase("1leaddec", "__1__leaddec")]
        [TestCase("-leaddefis", "__MINUS__leaddefis")]
        [TestCase("in.dot", "in.dot")]
        [TestCase(".leaddot", "__DOT__leaddot")]
        [TestCase("русское", "русское")]
        [TestCase("!a*", "__EXC__a__STAR__")]
        public void TestEscaping(string input, string output) {
            Assert.AreEqual(output,input.EscapeXmlName());
        }

        [TestCase("in-defis", "in-defis")]
        [TestCase("in1dec", "in1dec")]
        [TestCase("1leaddec", "__1__leaddec")]
        [TestCase("-leaddefis", "__DEF__leaddefis")]
        [TestCase("in.dot", "in.dot")]
        [TestCase(".leaddot", "__DOT__leaddot")]
        [TestCase("русское", "русское")]
        [TestCase("!a*", "__EXC__a__STAR__")]
        public void TestUnEscaping(string output, string input)
        {
            Assert.AreEqual(output, input.UnescapeXmlName());
        }
    }
}
