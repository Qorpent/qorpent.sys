using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl2;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.Bxl2
{
    class BxlParser2Tests
    {
        [Test]
        [Explicit]
        public void BxlParserTest() {
            String bxl = @"test q, ,,=, ,	,w
,qwerty

";

            BxlParser parser = new BxlParser();
            XElement res = parser.Parse(bxl, "ololo.txt", BxlParserOptions.NoLexData);
            Console.WriteLine(res);
        }

        [Test]
        public void CanParse() {
			String bxl = @"tes+t1 f f
	a    5
		-b	xx   =   4 xx=5
    c
		d
			e
test2
	f e
	g  r	k
	h	d,g,
";

			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);
        }

	    [Test]
	    public void CanGetAnonymousAttribute() {
			String bxl = @"
test1 a b c
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

		    XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("code")).Value, "a");
			Assert.AreEqual(test1.Attribute(XName.Get("id")).Value, "a");
			Assert.AreEqual(test1.Attribute(XName.Get("name")).Value, "b");
			Assert.AreEqual(test1.Attribute(XName.Get("c")).Value, "1");
	    }

	    [Test]
	    public void CanGetAttributeValue() {
			String bxl = @"
test1 x=1 y=2 x=3
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("x")).Value, "3");
			Assert.AreEqual(test1.Attribute(XName.Get("y")).Value, "2");
	    }

	    [Test]
	    public void CanSkipMultipleSpaces() {
			String bxl = @"
test1 x   =   1
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);
	    }
    }
}
