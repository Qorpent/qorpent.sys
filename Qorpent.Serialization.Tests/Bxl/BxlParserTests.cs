﻿using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.Bxl
{
    [TestFixture]
	public class BxlParserTests {
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

			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().Last().Attribute("_line").Value, "7");
		}

		[Test]
		public void PreventNameAnonymAfterAnyNamed(){
			var code = @"
e c x=1 n
";
			var b = new BxlParser().Parse(code,"",BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <e code='c' id='c' x='1' n='1' />
</root>", b.ToString().Replace("\"", "'"));
		}


		[Test]
		public void ValidlyParsesEmptyStringsBasis()
		{
			var code = @"
n g
	f s ""X""
		i c a=b
			p = """"
		i c a=b
			p = """"
";
			var b = new BxlParser().Parse(code, "", BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <n code='g' id='g'>
    <f code='s' id='s' name='X'>
      <i code='c' id='c' a='b' p='' />
      <i code='c' id='c' a='b' p='' />
    </f>
  </n>
</root>", b.ToSqlString().Replace("\"", "'"));
		}

		[Test]
public void TryFindBugInParsing()
		{
			var code = @"
namespace Graph.Sample.Tests
	fixture SP_MChRP_ORG ""Себестоимость продукции / МЕДЬ ЧЕРНОВАЯ. Расходы передела (предприятия)""
		h=${_hl}
		include call cls=^CuChernGraphRash 
			fixed = ""излишне опять делит на 1000  - realvalues не цепляет скины""	
		include call cls=^CuChernGraphObchras h=${_hl}
			problem = """"
";
			var b = new BxlParser().Parse(code, "", BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <namespace code='Graph.Sample.Tests' id='Graph.Sample.Tests'>
    <fixture code='SP_MChRP_ORG' id='SP_MChRP_ORG' name='Себестоимость продукции / МЕДЬ ЧЕРНОВАЯ. Расходы передела (предприятия)' h='${_hl}'>
      <include code='call' id='call' cls='^CuChernGraphRash' fixed='излишне опять делит на 1000  - realvalues не цепляет скины' />
      <include code='call' id='call' cls='^CuChernGraphObchras' h='${_hl}' problem='' />
    </fixture>
  </namespace>
</root>", b.ToSqlString().Replace("\"", "'"));
		}
		[Test]
		public void ValidlyParsesEmptyStrings(){
			var code = @"
x x='' y="""" z=""""""""""""  : """"
	a = """"
y a=1
";
			var b = new BxlParser().Parse(code, "", BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <x x='' y='' z='' a=''></x>
  <y a='1' />
</root>",b.ToSqlString().Replace("\"","'"));
		}

		[Test]
		public void NameAnonymAfterNoAnyNamed()
		{
			var code = @"
e c  n x=1
";
			var b = new BxlParser().Parse(code, "", BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <e code='c' id='c' name='n' x='1' />
</root>", b.ToString().Replace("\"", "'"));
		}

		[Test]
		public void NotOverrideId()
		{
			var code = @"
e id=1 c  n x=1
";
			var b = new BxlParser().Parse(code, "", BxlParserOptions.NoLexData);
			Console.WriteLine(b.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<root>
  <e id='1' code='c' name='n' x='1' />
</root>", b.ToString().Replace("\"", "'"));
		}

		[Test]
		public void CanGetAnonymousAttribute() {
			String bxl = @"
test1 a b c
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("code")).Value, "a");
			Assert.AreEqual(test1.Attribute(XName.Get("id")).Value, "a");
			Assert.AreEqual(test1.Attribute(XName.Get("name")).Value, "b");
			Assert.AreEqual(test1.Attribute(XName.Get("c")).Value, "1");
		}

		[Test]
		public void AnonCodeNotOverrideDirectlySetId(){
			String bxl = @"a id=1 a";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl).Elements().First();
			Console.WriteLine(res);
			Assert.AreEqual("1",res.Attr("id"));
		}

		[Test]
		public void CanGetAttributeValue() {
			String bxl = @"
test1 x=1 y=2 x=3
";
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("x")).Value, "2");
		}

		[Test]
		public void CanGetNestedAttribute() {
			String bxl = @"test1 x
	y = 5
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("y")).Value, "5");
		}

		[Test]
		public void CanUseSingleLineStringAsAnonAttribute() {
			String bxl = @"test1 a 'w w' ""q q"" (qwerty)";
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("x")).Value, "q q");
		}

		[Test]
		public void CanUseSingleLineStringAsAttributeName() {
			String bxl = @"test1 'w w' = ""q q""
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("w w".Escape(EscapingType.XmlName))).Value, "q q");
		}

		[Test]
		public void CanUseEscapedCharacters() {
			String bxl = @"test ""q\""q""=""r\""r""
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q\"q".Escape(EscapingType.XmlName))).Value, "r\"r");
		}

		[Test]
		public void CanUseUseEmptyStringInAttributeValue() {
			String bxl = @"test q=""""
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Attribute(XName.Get("q")).Value, "");
		}

		[Test]
		public void CanUseMultiLineStringAsAttributeValue() {
			String bxl = @"test q=""""""qwerty
asdf""""""
";
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("qwerty")).Value, "(\r\nnested (expression)\r\n)");
		}

		[Test]
		public void CanUseNestedDifferentBrakesExpression()
		{
			String bxl = @"test1 qwerty=(
nested (expression[x{2}])
)
";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("qwerty")).Value, "(\r\nnested (expression[x{2}])\r\n)");
		}

		[Test]
		public void CannotUseExtraEndExpressions()
		{
			String bxl = @"test1 qwerty=(
nested (expression[x{2}]))
)
";
			IBxlParser p = new BxlParser();
			Assert.Throws<BxlException>(() => p.Parse(bxl));

		}

		[Test]
		public void CannotUseExtraBeginExpressions()
		{
			String bxl = @"test1 qwerty=(((
(nested (expression[x{2}])
)
";
			IBxlParser p = new BxlParser();
			Assert.Throws<BxlException>(() => p.Parse(bxl));

		}


		[Test]
		public void CannotUseNestedIfBrokenBrakesExpression()
		{
			String bxl = @"test1 qwerty=(
nested (expression[x{2]})
)
";
			IBxlParser p = new BxlParser();
			Assert.Throws<BxlException>(() => p.Parse(bxl));

		}

		[Test]
		public void CanUseExprecssionAsAnonAttribute() {
			String bxl = @"test1 a 'w w' (q q)";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			Assert.AreEqual(test1.Attribute(XName.Get("name")).Value, "w w");
			Assert.AreEqual(test1.Attribute(XName.Get("_aa4")).Value, "(q q)");
		}

		[Test]
		public void CanUseTextContentSimpleLiteral() {
			String bxl = @"test1 : qwerty
";
			IBxlParser p = new BxlParser();
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
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Value, "qwerty'\r\n\t: 'ololo");
		}

		[Test]
		public void CanUseTextContentWithAttributes() {
			String bxl = @"test1 a b c=3:qwerty
";
			IBxlParser p = new BxlParser();
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

			IBxlParser p = new BxlParser();
			foreach (string code in bxl) {
				XElement res = p.Parse(code);
				Console.WriteLine(res);

				Assert.AreEqual(res.Elements().First().Name.LocalName, "test1");
				Assert.AreEqual(res.Elements().First().Value, "qwerty");
				Assert.AreEqual(res.Elements().First().Elements().First().Name.LocalName, "test2");
			}
		}

		[Test]
		public void CanDeclareNamespace() {
			String bxl = @"ns1=qwerty
""""""ns
2""""""=""""""ololo""""""
test";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.GetNamespaceOfPrefix("ns1").NamespaceName, "qwerty");
			Assert.AreEqual(res.GetNamespaceOfPrefix("ns\r\n2".Escape(EscapingType.XmlName)).NamespaceName, "ololo");
		}

		[Test]
		public void CanProcessExtraTabs() {
			String bxl = @"	test1
	test2";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			Assert.AreEqual(res.Elements().First().Name.LocalName, "test1");
			Assert.AreEqual(res.Elements().Last().Name.LocalName, "test2");
		}

		[Test]
		public void CanUseElementNamespace() {
			String bxl = @"ns1=qwerty
ns2=qwerty2
ns1::test1
	ns2::test2";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			XElement test2 = test1.Elements().First();

			Assert.AreEqual(test1.Name.LocalName, "test1");
			Assert.AreEqual(test1.Name.NamespaceName, "qwerty");
			Assert.AreEqual(test2.Name.LocalName, "test2");
			Assert.AreEqual(test2.Name.NamespaceName, "qwerty2");
		}

		[Test]
		public void CanUseAttributeNamespace() {
			String bxl = @"ns1=qwerty
ns2=qwerty2
test1 ns1::x=2 ns2::y=3";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl, "file", BxlParserOptions.NoLexData);
			Console.WriteLine(res);

			XElement test1 = res.Elements().First();
			XAttribute att1 = test1.Attributes().First();
			XAttribute att2 = test1.Attributes().Last();

			Assert.AreEqual(att1.Name.LocalName, "x");
			Assert.AreEqual(att1.Name.NamespaceName, "qwerty");
			Assert.AreEqual(att2.Name.LocalName, "y");
			Assert.AreEqual(att2.Name.NamespaceName, "qwerty2");
		}

		[Test]
		public void CanUseAnonAttributeNamespace() {
			String bxl = @"ns1=qwerty
test a b ns1::x";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test = res.Elements().First();
			XAttribute att = test.Attributes().Last();

			Assert.AreEqual(att.Name.LocalName, "x");
			Assert.AreEqual(att.Name.NamespaceName, "qwerty");
		}

		[Test]
		public void CanDeclareDefaultNamespace() {
			String bxl = @"ns1=qwerty
ns2::test a b ns3::x";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement test = res.Elements().First();
			XAttribute att = test.Attributes().Last();

			Assert.AreEqual(test.Name.NamespaceName, "namespace::code.bxl_X");
			Assert.AreEqual(att.Name.NamespaceName, "namespace::code.bxl_XX");
		}

		[Test]
		public void CanSkipCommentary() {
			String bxl = @"#qwerty
ns1=qwerty #qwerty
ns2::test a b ns3::x
#qwerty
test
#qwerty";
			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);
		}

		[Test]
		public void CanUseOptions() {
			String bxl = @"test1 a b c";

			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl, "qqqq", BxlParserOptions.NoLexData | BxlParserOptions.OnlyIdAttibute | BxlParserOptions.SafeAttributeNames | BxlParserOptions.ExtractSingle);
			Console.WriteLine(res);
			Assert.AreEqual(res.Attributes().First().Name.LocalName, "__id");
		}

		[Test]
		public void CanCountAnonAttributes() {
			String bxl = @"
test1 a b
	test2 c
	'anon1'
		'anon2'";

			IBxlParser p = new BxlParser();
			XElement res = p.Parse(bxl);
			Console.WriteLine(res);

			XElement t1 = res.Element("test1");
			XElement t2 = t1.Element("test2");

			Assert.AreEqual(t1.Attribute("_aa4").Value, "anon1");
			Assert.AreEqual(t2.Attribute("name").Value, "anon2");
		}


		[TestCase("demo.import.forms.m600.bxls")]
		[TestCase("presentation_ocm_structure.hql")]
		public void HardTest(String filename) {

			String bxl = GetType().Assembly.ReadManifestResource(filename);
			var xml1 = new BxlParser().Parse(bxl);
		}
	}
}
