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
	internal class BxlParser2Tests {
		[Test]
		[Explicit]
		public void BxlParserTest() {
			String bxl = @"test s=""""""\\""""""
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
test1 x   =   2
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("x")).Value, "2");
		}

		[Test]
		public void CanGetNestedAttribute() {
			String bxl = @"test1 x
	y=5
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("y")).Value, "5");
		}

		[Test]
		public void CanUseSingleLineStringAsAnonAttribute() {
			String bxl = @"test1 a 'w w' 'q q'
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("name")).Value, "w w");
			Assert.AreEqual(test1.Attribute(XName.Get("_aa4")).Value, "q q");
		}

		[Test]
		public void CanUseSingleLineStringAsAttributeValue() {
			String bxl = @"test1 x='q q'
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("x")).Value, "q q");
		}

		[Test]
		public void CanUseSingleLineStringAsAttributeName() {
			String bxl = @"test1 'w w' = 'q q'
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("w w".Escape(EscapingType.XmlName))).Value, "q q");
		}

		[Test]
		public void CanUseEscapedCharacters() {
			String bxl = @"test ""q\""q""=""r\""r""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q\"q".Escape(EscapingType.XmlName))).Value, "r\"r");
		}

		[Test]
		public void CanUseUseEmptyStringInAttributeValue() {
			String bxl = @"test q=""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q".Escape(EscapingType.XmlName))).Value, "");
		}

		[Test]
		public void CanUseMultiLineString() {
			String bxl = @"test s=""""""qwerty
asdf""""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			//Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q".Escape(EscapingType.XmlName))).Value, "");
		}
	}
}
