#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : BxlParserTest.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.Bxl {
	[TestFixture]
	public class BxlParserTest {
		[Test]
		public void CanBuildBxlTokenTree() {
			var parser = new BxlParser();
			var tree =
				parser.ParseTree(
					@"
root code, name y=33 : 'value'
	attr2 = 4
	child code2 : 'val2'
		child c1 , x=1
		child c2
		child c3, x=2
		child c4
")
					.Tokens;
			Assert.AreEqual(1, tree.Length);
			Assert.AreEqual("root", tree[0].Value);
			Assert.AreEqual(1, tree[0].Elements.Count);
			Assert.AreEqual(4, tree[0].Attributes.Count);
			Assert.AreEqual("value", tree[0].ElementValue.Value);
			Assert.AreEqual(4, tree[0].Elements[0].Elements.Count);
			Assert.AreEqual(tree[0], tree[0].Elements[0].Parent);
		}

		[Test]
		public void CanInterpolateDuringParse() {
			var res = new BxlParser().Parse(@"
test x='1' y=3 
	test2 x='${.x}${y}2' 
		test3  y='${x}${.y}'
	", "", BxlParserOptions.PerformInterpolation);
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root>
  <test _file=""code.bxl"" _line=""2"" x=""1"" y=""3"">
    <test2 _file=""code.bxl"" _line=""3"" x=""132"">
      <test3 _file=""code.bxl"" _line=""5"" y=""1323"" />
    </test2>
  </test>
</root>",res.ToString());
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
		[Ignore("now implicit namespaces supported, so this case is valid variant of implicit namespacing")]
		public void ErrorOnIllegalNameSpaces() {
			Assert.Throws<BxlException>(
				() => new BxlParser().Parse(@"
#no valid namespace
ns2 = 'x'
ns1::test
")
				);
			//NOTE - it's hack to provide text to root level - will think what to do
			/*	Assert.Throws<BxlParserException>(
				() => {
				      	var x = new BxlXmlParser().Parse(@"
#ns at start
ns1 = 'x'
::test
");
					Console.WriteLine(x.ToString());
				}
				);*/
			Assert.Throws<BxlException>(
				() => new BxlParser().Parse(@"
#ns at end
ns1 = 'x'
test::
")
				);

			Assert.Throws<BxlException>(
				() => new BxlParser().Parse(@"
#tripple ns
ns1 = 'x'
ns1:::test
")
				);
		}

		[Test]
		public void GeneratesNamespaceDeclarations() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
test");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"">
  <test _file=""code.bxl"" _line=""3"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void GeneratesNamespaceDeclarations_ImplicitWellknownNameSpace() {
			var res = new BxlParser().Parse(@"
e1 :
	qxi::include direct//inclA
	qxi::include direct//inclD delay
	qxi::import direct//impA	
	qxi::import direct//impD delay");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:qxi=""http://qorpent/xml/include"">
  <e1 _file=""code.bxl"" _line=""2"">
    <qxi:include _file=""code.bxl"" _line=""3"" code=""direct//inclA"" id=""direct//inclA"" />
    <qxi:include _file=""code.bxl"" _line=""4"" code=""direct//inclD"" id=""direct//inclD"" name=""delay"" />
    <qxi:import _file=""code.bxl"" _line=""5"" code=""direct//impA"" id=""direct//impA"" />
    <qxi:import _file=""code.bxl"" _line=""6"" code=""direct//impD"" id=""direct//impD"" name=""delay"" />
  </e1>
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void GeneratesNamespaceDeclarations_NotWorkingExample() {
			var res = new BxlParser().Parse(@"
qxi='http://qorpent/xml/include'
e1 :
	qxi::include direct//inclA
	qxi::include direct//inclD delay
	qxi::import direct//impA	
	qxi::import direct//impD delay");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:qxi=""http://qorpent/xml/include"">
  <e1 _file=""code.bxl"" _line=""3"">
    <qxi:include _file=""code.bxl"" _line=""4"" code=""direct//inclA"" id=""direct//inclA"" />
    <qxi:include _file=""code.bxl"" _line=""5"" code=""direct//inclD"" id=""direct//inclD"" name=""delay"" />
    <qxi:import _file=""code.bxl"" _line=""6"" code=""direct//impA"" id=""direct//impA"" />
    <qxi:import _file=""code.bxl"" _line=""7"" code=""direct//impD"" id=""direct//impD"" name=""delay"" />
  </e1>
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
		public void NotAllowAttributesAtRoot() {
			Assert.Throws<BxlException>(() => new BxlParser().Parse(@"
test 
x=2"));
		}

		[Test]
		[Explicit]
		public void ParserBenchMark() {
			var xml = new XElement("root");
			for (var i = 0; i <= 100; i++) {
				var e = new XElement("child", new XAttribute("code", i), i);
				for (var j = 0; j <= 100; j++) {
					var e2 = new XElement("child2", new XAttribute("code", "2_" + j), "2_" + j);
					e.Add(e2);
				}
				xml.Add(e);
			}
			var tmp = Path.GetTempFileName();
			xml.Save(tmp + "__test__.xml");
			File.WriteAllText(tmp + "__test__.bxl", new BxlGenerator().Convert(xml, null));
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < 10; i++) {
				XElement.Load(tmp + "__test__.xml");
			}
			sw.Stop();
			var xmltime = sw.ElapsedMilliseconds;
			Console.WriteLine(xmltime);
			sw = Stopwatch.StartNew();
			for (var i = 0; i < 10; i++) {
				new BxlParser().ParseTokens(File.ReadAllText(tmp + "__test__.bxl"));
			}
			sw.Stop();
			var bxltime = sw.ElapsedMilliseconds;
			Console.WriteLine(bxltime);
			Console.WriteLine(bxltime - xmltime);
			Assert.True(bxltime < xmltime);
		}

		[Test]
		public void SupportBracesInLiteral() {
			var res = new BxlParser().Parse("test val(1)");
			Console.WriteLine(res.ToString());
		}

		[Test]
		public void TryXpathNavigation() {
			var nav =
				new BxlParser().ParseTree(
					@"
root code, name y=33 : 'value'
	attr2 = 4
	child code2 : 'val2'
		child c1 , x=1
		child c2
		child c3, x=2
		child c4
")
					.CreateNavigator();
			var iter = nav.Select("//child/child[@x]");
			Assert.AreEqual(2, iter.Count);
			iter.MoveNext();
			if (iter.Current != null) {
				Assert.AreEqual("c1", iter.Current.GetAttribute("code", ""));
			}
			iter.MoveNext();
			if (iter.Current != null) {
				Assert.AreEqual("c3", iter.Current.GetAttribute("code", ""));
			}
		}

		[Test]
		public void UsesNamespacesInAttributeNames() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
ns::test ns::attr=2");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"">
  <ns:test _file=""code.bxl"" _line=""3"" ns:attr=""2"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void UsesNamespacesInElementNames() {
			var res = new BxlParser().Parse(@"
ns = 'http://myns'
ns::test");
			Console.WriteLine(res.ToString());
			Assert.AreEqual(@"<root xmlns:ns=""http://myns"">
  <ns:test _file=""code.bxl"" _line=""3"" />
</root>".LfOnly(), res.ToString().LfOnly());
		}

		[Test]
		public void WrongAssignError() {
			var exc = Assert.Throws<BxlException>(() =>
			                                      new BxlParser().ParseTokens("x (a)=2"));
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
            Assert.AreEqual("a:a",a.Value);
        }
	}
}