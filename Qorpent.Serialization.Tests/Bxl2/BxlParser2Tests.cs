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
		public void BxlParserLevelTextContent() {
			// текстовый контент для узлов определяется почему-то без учета отступов
			String bxl = @"test1
	test3
:qwerty
		test2
";

			BxlParser parser = new BxlParser();
			XElement res = parser.Parse(bxl, "ololo.txt", BxlParserOptions.NoLexData);
			Console.WriteLine(res);
		}

		[Test]
		[Explicit]
		public void BxlParsertest() {
			// текстовый контент для узлов определяется почему-то без учета отступов
			String bxl = @"ss='qwerty'
ss::test
qq::test
ww::test";

			BxlParser parser = new BxlParser();
			XElement res = parser.Parse(bxl, "ololo.txt", BxlParserOptions.NoLexData);
			Console.WriteLine(res);
		}

		[Test]
		[Explicit]
		public void AnyTest() {
			String bxl = @"	test
	k=ololo";

			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
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
			String bxl = @"test1 a 'w w' ""q q""
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
			String bxl = @"test1 'w w' = ""q q""
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

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q")).Value, "");
		}

		[Test]
		public void CanUseMultiLineStringAsAttributeValue() {
			String bxl = @"test q=""""""qwerty
asdf""""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q")).Value, "qwerty\r\nasdf");
		}

		[Test]
		public void CanUseMultiLineStringAsAnonAttribute() {
			String bxl = @"test1 a """"""1
2 ' ''' """"
3""""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("name")).Value, "1\r\n2 ' ''' \"\"\r\n3");
		}

		[Test]
		public void CanUseMultiLineStringAsAttributeName() {
			String bxl = @"test1 a
	""""""q
w
e"""""" = """"""r
t
y""""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("q\r\nw\r\ne".Escape(EscapingType.XmlName))).Value, "r\r\nt\r\ny");
		}

		[Test]
		public void CanUseExpression() {
			String bxl = @"test1 qwerty=(
nested (expression)
)
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("qwerty")).Value, "(\r\nnested (expression)\r\n)");
		}

		[Test]
		public void CanUseTextContentSimpleLiteral() {
			String bxl = @"test1 : qwerty
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Value, "qwerty");
		}

		[Test]
		public void CanUseTextContentMultilineString() {
			String bxl = @"test1 
	: """"""qwerty'
	: 'ololo""""""
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Value, "qwerty'\r\n\t: 'ololo");
		}

		[Test]
		public void CanUseTextContentWithAttributes() {
			String bxl = @"test1 a b c=3:qwerty
";
			IBxlParser p = new BxlParser2();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("name")).Value, "b");
			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("c")).Value, "3");
			Assert.AreEqual(res.Elements().First().Value, "qwerty");
		}

		[Test]
		public void CanUseTextContentWithChildElements() {
			String[] bxl = {
@"test1 
	:qwerty
	test2",
@"test1 
	test2
	:qwerty"
			};

			IBxlParser p = new BxlParser2();
			foreach (string code in bxl) {
				XElement res = p.Parse(code);
				Console.WriteLine(res);

				Assert.AreEqual(res.Elements().First().Name.LocalName, "test1");
				Assert.AreEqual(res.Elements().First().Value, "qwerty");
				Assert.AreEqual(res.Elements().First().Elements().First().Name.LocalName, "test2");
			}
		}
	}
}
