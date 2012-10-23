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
// Original file : BxlTokenizerTestComplex.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Xml.Linq;
using Comdiv.UXmlDiff;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

#if !SQL2008
#endif

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class BxlParserBug_got_in_assoieco_file {
		private const string problemcode = @"   
a x = ?x
	b +x
		c ?x
	z=0
    y=/x
    ";
		private const string problemcode2 = @"
a :
	b :
		c :
	x=0";

		private const string problemcode_final = @"
th t :
	rs t2 :
		NULL #comment

#comment
	fnB =""x""";

#if !SQL2008
		[Test]
		public void xml_generation_1() {
			var x = new BxlParser().Parse(problemcode, options: BxlParserOptions.NoLexData);
			var diff = new XDiffExecutor(@"
<root>
  <a x='?x' z='0' y='/x'>
    <b code='+x' id='+x'>
      <c code='?x' id='?x' />
    </b>
  </a>
</root>
", x).Execute();
			processdiff(x, diff);
		}

		[Test]
		public void xml_generation_final() {
			var x = new BxlParser().Parse(problemcode_final, options: MyBxl.GetOptions(true));
			var diff = new XDiffExecutor(@"
<root>
<th fnB='x' id='t' code='t'>
	<rs id='t2' code='t2'  >
		<NULL />
	</rs>
</th>
</root>
", x).Execute();
			if (diff.Length > 0) {
				Console.WriteLine(x);
				foreach (var a in diff) {
					Console.WriteLine(a);
				}
				Assert.Fail("ошибка парсинга");
			}
		}

		private void processdiff(XElement x, XDiffAnnotation[] diff) {
			if (diff.Length > 0) {
				Console.WriteLine(x);
				foreach (var a in diff) {
					Console.WriteLine(a);
				}
				Assert.Fail("ошибка парсинга");
			}
		}
#endif

		[Test]
		public void get_tokens_1() {
			var tokens = new BxlTokenizer().Tokenize(problemcode);
			var sn = TokenizerTestHelper.GetShortTypedNotation(tokens, "_");
			Assert.AreEqual(
				@"{0_EA_1EL_2EL_1A_1A}", sn);
		}

		[Test]
		public void strange_attr_behavior() {
			new BxlTokenizer().Tokenize("thema balans \"Баланс 2011\" role=xx");
			new BxlTokenizer().Tokenize("thema code, param=kkkk");
			Assert.Throws<BxlException>(() => new BxlTokenizer().Tokenize("thema balans \"Баланс 2011\" role="));
		}

		[Test]
		[Ignore("new adaptive literal checking")]
		public void error_number_as_attribute() {
			var m = Assert.Throws<Exception>(() => new BxlTokenizer().Tokenize("a 1=x")).Message;
			StringAssert.Contains("cannot generate attribute from Number and Literal", m);
		}

		[Test]
		[Ignore("new adaptive literal checking")]
		public void non_finished_attribute() {
			var m = Assert.Throws<Exception>(() => new BxlTokenizer().Tokenize(@"
a 1= :
    x
")).Message;
			StringAssert.Contains("cannot generate attribute from Number and Colon at", m);
		}

		[Test]
		public void get_tokens_note_used_symbols_for_cover_1() {
			var tokens = new BxlTokenizer().Tokenize(problemcode);
			var sn = TokenizerTestHelper.GetShortTypedNotation(tokens, "_");
			Assert.AreEqual(
				@"{0_EA_1EL_2EL_1A_1A}"
				, sn);
		}

		[Test]
		public void get_tokens_2() {
			var tokens = new BxlTokenizer().Tokenize(problemcode2);
			var sn = TokenizerTestHelper.GetShortTypedNotation(tokens, "_");
			Assert.AreEqual(
				@"{E_1E_2E_1A}", sn);
		}

		[Test]
		public void get_tokens_final() {
			var tokens = new BxlTokenizer().Tokenize(problemcode_final);
			var sn = TokenizerTestHelper.GetShortTypedNotation(tokens, "_");
			Assert.AreEqual(
				@"{EL_1EL_2E_1A}", sn);
		}
	}

	[TestFixture]
	public class BxlTokenizerTestComplex {
		[Test]
		[Ignore("new adaptive literal checking")]
		public void EXCEPTION_illegal_attribute() {
			Assert.Throws<Exception>(() => new BxlTokenizer().Tokenize("el 1=2"))
				.Message.Contains("attribute");
		}

		[Test]
		public void EXCEPTION_illegal_value() {
			Assert.Throws<BxlException>(() => new BxlTokenizer().Tokenize("el : 1 2"))
				.Message.Contains("declaration");
		}


		[Test]
		public void EXCEPTION_multiline_with_n_only() {
			var tokens = new BxlTokenizer().Tokenize("\n\n\nel");
			Assert.AreEqual(3, tokens.Length);
			Assert.AreEqual(4, tokens[1].LexInfo.Line);
		}

		[Test]
		[Ignore("new adaptive literal checking")]
		public void EXCEPTION_must_start_with_literal() {
			Assert.Throws<Exception>(() => new BxlTokenizer().Tokenize("23 : 1 "))
				.Message.Contains("element");
		}

		[Test]
		[Ignore("new parsr behavior to provide empty value in such cases")]
		public void EXCEPTION_not_closed_colon() {
			Assert.Throws<Exception>(() => new BxlTokenizer().Tokenize("el : "))
				.Message.Contains("colon");
		}

		[Test]
		public void bad_expression() {
			var code = @"x (y == a && z='ddd'
|| (a!= 3)";

			var ex = Assert.Throws<BxlException>(() => new BxlTokenizer().Tokenize(code));
			StringAssert.Contains("not closed expression", ex.Message);
		}


		[Test]
		public void big_expression() {
			var code = @"x (y == a && z='ddd'
|| (a!= 3))";
			Assert.AreEqual("{EX}", TokenizerTestHelper.GetShortTypedNotation(new BxlTokenizer().Tokenize(code)));
			Assert.AreEqual(code.Substring(2), new BxlTokenizer().Tokenize(code)[2].Value);
		}

		[Test]
		public void big_expression_in_xml() {
			var code = @"x (y == a && z='ddd'
|| (a!= 3))";
			var xml = new BxlParser().Parse(code).ToString();
			Console.WriteLine(xml);
			Assert.AreEqual(@"<root>
  <x _file=""code.bxl"" _line=""1"" code=""(y == a &amp;&amp; z='ddd'&#xD;&#xA;|| (a!= 3))"" id=""(y == a &amp;&amp; z='ddd'&#xD;&#xA;|| (a!= 3))"" />
</root>".LfOnly(), xml.LfOnly());
		}

		[Test]
		public void big_expression_in_xml_value() {
			var code = @"x : (y == a && z='ddd'
|| (a!= 3))";
			var xml = new BxlParser().Parse(code).ToString();
			Console.WriteLine(xml);
			Assert.AreEqual(@"<root>
  <x _file=""code.bxl"" _line=""1"">(y == a &amp;&amp; z='ddd'
|| (a!= 3))</x>
</root>".LfOnly(), xml.LfOnly());
		}

		[Test]
		public void braces_validly_catched_in_strings() {
			Assert.AreEqual("{ES}", TokenizerTestHelper.GetShortTypedNotation(new BxlTokenizer().Tokenize("x '))(('")));
		}

		[Test]
		public void multilevel_valid_code_with_all_elements() {
			var tokens = new BxlTokenizer().Tokenize(
				@"
#comment
e1 code1, 'name 1', x.z = 1 : val #comment
	e11 code2 
		y = 35
	e12 code3
	x = 34"
				);
			Assert.AreEqual(
				@"{ELSAV
1EL
2A
1EL
1A}", TokenizerTestHelper.GetShortTypedNotation(tokens)
				);
		}


		[Test]
		public void multilevel_valid_code_with_all_elements_and_tripples() {
			var tokens = new BxlTokenizer().Tokenize(
				@"
#comment
e1 code1, """"""name 
	1"""""", x = 1 : val #comment
	e11 code2 
		y = 35
	e12 code3
	x = """"""34
dsds"""""""
				);
			Assert.AreEqual(
				@"{ELSAV
1EL
2A
1EL
1A}", TokenizerTestHelper.GetShortTypedNotation(tokens)
				);
		}

		[Test]
		public void no_parse_value_on_next_line_BUG() {
			new BxlTokenizer().Tokenize(
				@"item :
	""""""
dasdsadsa
""""""
			");
		}

		[Test]
		public void simple_expression() {
			Assert.AreEqual("{EX}", TokenizerTestHelper.GetShortTypedNotation(new BxlTokenizer().Tokenize("x (y)")));
		}
	}
}