using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Comdiv.UXmlDiff;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.Bxl {
	class BxlParserCompatibilityTests {
		[Test]
		public void CanInterpolateDuringParse() {
			var res = new BxlParser().Parse(@"
test x='1' y=3 
	test2 x='${.x}${y}2' 
		test3  y='${x}${.y}'
	", "", BxlParserOptions.PerformInterpolation);
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root _file=""code.bxl"">
  <test _file=""code.bxl"" _line=""2"" x=""1"" y=""3"">
    <test2 _file=""code.bxl"" _line=""3"" x=""132"">
      <test3 _file=""code.bxl"" _line=""4"" y=""1323"" />
    </test2>
  </test>
</root>", res.ToString());
		}

		[Test]
		public void CanUseBSharpDuringParse() {
			var res = new BxlParser().Parse(@"
class A abstract
	x=1
class B
	import A
	y='${x}'
	", "", BxlParserOptions.BSharp);
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<bsharp>
  <class code=""B"" y=""${x}"" fullcode=""B"" x=""1"" />
</bsharp>", res.ToString());
		}

		[Test]
		public void CanUseBSharpWithInterpolationsDuringParse() {
			var res = new BxlParser().Parse(@"
class A abstract
	x=1
class B
	import A
	y='${x}'
	", "", BxlParserOptions.BSharp | BxlParserOptions.PerformInterpolation);
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<bsharp>
  <class code=""B"" y=""1"" fullcode=""B"" x=""1"" />
</bsharp>", res.ToString());
		}

		[Test]
		public void CanParseAttribuesAfterElements() {
			var res = new BxlParser().Parse(@"
test 
	test2
	y=3
		test3
	x=2");
			Console.WriteLine(res.ToString());
		}

		[Test]
		public void GeneratesNamespaceDeclarations() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
test");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"" _file=""code.bxl"">
  <test _file=""code.bxl"" _line=""3"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}



		[Test]
		public void Implicit_Namespace_Generation() {
			var res = new BxlParser().Parse(@"
a::e1
b::e1", "myfile", BxlParserOptions.NoLexData);
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:a=""namespace::myfile_X"" xmlns:b=""namespace::myfile_XX"">
  <a:e1 />
  <b:e1 />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		[Ignore("now it's allowed")]
		public void NotAllowAttributesAtRoot() {
			Assert.Throws<BxlException>(() => Console.WriteLine( new BxlParser().Parse(@"
test 
x=2")));
		}

		[Test]
		public void SupportBracesInLiteral() {
			var res = new BxlParser().Parse("test val(1)");
			Console.WriteLine(res.ToString());
		}

		[Test]
		public void UsesNamespacesInAttributeNames() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
ns::test ns::attr=2");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"" _file=""code.bxl"">
  <ns:test _file=""code.bxl"" _line=""3"" ns:attr=""2"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void UsesNamespacesInElementNames() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
ns::test");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"" _file=""code.bxl"">
  <ns:test _file=""code.bxl"" _line=""3"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void WrongAssignError() {
			var exc = Assert.Throws<BxlException>(() =>
												  new BxlParser().Parse("x (a)=2"));
			StringAssert.Contains("assign", exc.Message);
		}

		[TestCase("e x=\"\"\"a:a\"\"\"")]
		[TestCase("e\r\n\tx=\"\"\"a:a\"\"\"")]
		[TestCase("e x='a:a'")]
		[TestCase("e\r\n\tx='a:a'")]
		[TestCase("e x=\"a:a\"")]
		public void QPT78_Not_Supported_DoubleDot_In_Attribute(string code) {

			var parsed = new BxlParser().Parse(code);
			var e = parsed.Element("e");
			var a = e.Attribute("x");
			Assert.NotNull(a);
			Assert.AreEqual("a:a", a.Value);
		}

		
		const int NamespaceCount = 10;
		const int MaxNameLength = 8;
		const int NodePerLevelCount = 4;
		const int AttributeCount = 8;
		const int Depth = 5;
			
		[Test]
		[Explicit]
		public void ParserBenchmark() {
			var xml = new XElement("root");
			for (int i = 0; i < NamespaceCount; i++) {
				String k = randomString();
				String v = randomString();
				ns[i] = v;
				xml.Add(new XAttribute(XNamespace.Xmlns + k, v));
			}
			for (var i = 0; i < NodePerLevelCount; i++) {
				xml.Add(createElement(Depth));
			}
			String xmlcode = xml.ToString();
			//Console.WriteLine(xmlcode);
			Console.WriteLine("xml length: {0}", xmlcode.Length);
			String bxlcode = new BxlGenerator().Convert(xml);
			//Console.WriteLine(bxlcode);
			Console.WriteLine("bxl length: {0}", bxlcode.Length);

			var sw = Stopwatch.StartNew();
		    for (var i = 0; i < 20; i++) {
		        XElement.Parse(xmlcode);
		    }
		    sw.Stop();
			var xmltime = sw.ElapsedMilliseconds;

			sw = Stopwatch.StartNew();
		    for (var i = 0; i < 20; i++) {
		        new BxlParser().Parse(bxlcode);
		    }
		    sw.Stop();
			var bxl1time = sw.ElapsedMilliseconds;

			sw = Stopwatch.StartNew();
		    for (var i = 0; i < 20; i++) {
		        new BxlParser().Parse(bxlcode);
		    }
		    sw.Stop();
			var bxl2time = sw.ElapsedMilliseconds;

			Console.WriteLine("xml: {0}\nbxl1: {1}\nbxl2: {2}", xmltime, bxl1time, bxl2time);
			Assert.True(bxl2time < bxl1time);
		}


        [Test]
        [Explicit]
        public void Parser2ProfileBenchmark()
        {
            var xml = new XElement("root");
            for (int i = 0; i < NamespaceCount; i++)
            {
                String k = randomString();
                String v = randomString();
                ns[i] = v;
                xml.Add(new XAttribute(XNamespace.Xmlns + k, v));
            }
            for (var i = 0; i < NodePerLevelCount; i++)
            {
                xml.Add(createElement(Depth));
            }
            String xmlcode = xml.ToString();
            //Console.WriteLine(xmlcode);
            Console.WriteLine("xml length: {0}", xmlcode.Length);
            String bxlcode = new BxlGenerator().Convert(xml);
            //Console.WriteLine(bxlcode);
            Console.WriteLine("bxl length: {0}", bxlcode.Length);
           
            for (var i = 0; i < 100; i++)
            {
                new BxlParser().Parse(bxlcode);
            }

           
        }

		private XElement createElement(int depth) {
			XElement e = new XElement(randomName());
			for (int i = 0; i < AttributeCount; i++) {
				switch (r.Next() & 3) {
					case 0:
						e.SetAttributeValue(randomName(), randomString());
						break;
					case 1:
						e.SetAttributeValue(randomName(), "1");
						break;
					case 2:
						e.SetAttributeValue("code", randomString());
						break;
					case 3:
						e.SetAttributeValue("name", randomString());
						break;
				}
			}
			if (depth > 0)
				for (var i = 0; i < NodePerLevelCount; i++)
					e.Add(createElement(depth - 1));
			return e;
		}

		private String randomString() {
			StringBuilder sb = new StringBuilder(r.Next(1, MaxNameLength));
			for (int i = 0; i < sb.Capacity; i++)
				sb.Append((char) r.Next('a', 'z'));
			return sb.ToString();
		}

		private XName randomName() {
			String _ns = (r.Next() & 1) == 0 ? ns[r.Next(0, NamespaceCount)] : "";
			return XName.Get(randomString(), _ns);
		}

		Random r = new Random();
		string[] ns = new string[NamespaceCount];
	}
}
