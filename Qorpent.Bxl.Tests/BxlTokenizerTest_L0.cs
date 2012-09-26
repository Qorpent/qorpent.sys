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
// Original file : BxlTokenizerTest_L0.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class BxlTokenizerTest_L0 {
		private void test(string code, object val, BxlTokenType type, int line, int col, int length) {
			var tokens = new BxlTokenizer().NoElements.Tokenize(code);
			Assert.AreEqual(3, tokens.Length);
			var token = tokens[1];
			Assert.AreEqual(type, token.Type);
			if (val is string) {
				Assert.AreEqual(val.ToString(), token.Value);
			}
			else {
				Assert.AreEqual(Convert.ToDecimal(val), token.Number);
			}
			Assert.AreEqual(line, token.LexInfo.Line);
			Assert.AreEqual(col, token.LexInfo.Column);
			Assert.AreEqual(length, token.LexInfo.Length);
		}

		[Test]
		public void EXCEPTION_illegal_escape_at_end_of_file() {
			Assert.Throws<BxlException>(() => test("\\", "str\r\nstr", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("end of file");
		}

		[Test]
		public void EXCEPTION_illegal_escape_not_in_string() {
			Assert.Throws<BxlException>(() => test("\\ \"zz\" ", "str\r\nstr", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("string");
		}


		[Test]
		public void EXCEPTION_illegal_literal_test() {
			Assert.Throws<BxlException>(() => test("liter'al", "literal", BxlTokenType.Literal, 1, 1, 7))
				.Message.Contains("illegal");
			Assert.Throws<BxlException>(() => test("liter\"al", "literal", BxlTokenType.Literal, 1, 1, 7))
				.Message.Contains("illegal");
		}

		[Test]
		public void EXCEPTION_illegal_literal_test_q() {
			Assert.Throws<BxlException>(() => test("literal\"", "literal", BxlTokenType.Literal, 1, 1, 7))
				.Message.Contains("illegal");
		}

		[Test]
		[Ignore("new adaptive literal checking")]
		public void EXCEPTION_illegal_number() {
			Assert.Throws<BxlException>(() => test("123.34.44", "", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("dot");
		}


		[Test]
		public void EXCEPTION_illegal_number_q() {
			Assert.Throws<BxlException>(() => test("123\"", "", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("dot");
		}


		[Test]
		[Ignore("new adaptive literal checking")]
		public void EXCEPTION_illegal_number_tobig() {
			Assert.Throws<BxlException>(() => test("123999999999999999999999999999934.44", "", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("number");
		}

		[Test]
		[Ignore("new adaptive literal checking")]
		public void EXCEPTION_must_close_number() {
			Assert.Throws<BxlException>(() => test("123.", "", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("not closed");
		}

		[Test]
		public void EXCEPTION_must_close_string() {
			Assert.Throws<BxlException>(() => test("\"str", "", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("not closed");
		}

		[Test]
		public void EXCEPTION_not_allow_newlines_in_usual_strings() {
			Assert.Throws<BxlException>(() => test("\"str\r\nstr\"", "str\r\nstr", BxlTokenType.String, 1, 1, 8))
				.Message.Contains("newline");
		}

		[Test]
		public void EXCEPTION_strings_must_have_ws_between_test() {
			Assert.Throws<BxlException>(() => test("\"1\"\"2\"", "literal", BxlTokenType.Literal, 1, 1, 7))
				.Message.Contains("delimiter");
		}

		[Test]
		public void comma_ignorance() {
			var x = new BxlParser().Parse("x a=1 b=2");
			Assert.AreEqual(@"<root>
  <x _file=""code.bxl"" _line=""1"" a=""1"" b=""2"" />
</root>", x.ToString());
//			Console.WriteLine(x);
		}

		[Test]
		public void complex_valid_test() {
			var tokens = new BxlTokenizer().Tokenize(
				@"element code, 'name' a1=v1  a2='v2' a3=-456 aa1 aa2 : 'value'"
				);
			Assert.AreEqual(11, tokens.Length);
			Assert.AreEqual("{ELSAAALLV}", TokenizerTestHelper.GetShortTypedNotation(tokens));
		}

		[Test]
		public void single_literal_test() {
			test("literal", "literal", BxlTokenType.Literal, 1, 1, 7);
		}

		[Test]
		public void single_string_apos_q_test() {
			test("'\"str\"'", "\"str\"", BxlTokenType.String, 1, 1, 7);
		}

		[Test]
		public void single_string_apos_test() {
			test("'str'", "str", BxlTokenType.String, 1, 1, 5);
		}

		[Test]
		public void single_string_cover_test() {
			test("'1234567890`~!@#$%^&*()_+'", "1234567890`~!@#$%^&*()_+", BxlTokenType.String, 1, 1, 26);
		}

		[Test]
		public void single_string_empty_string_test() {
			test("''", "", BxlTokenType.String, 1, 1, 2);
		}


		[Test]
		public void single_string_multiline_test() {
			test("\"\"\"str\r\nstr\"\"\"", "str\r\nstr", BxlTokenType.String, 1, 1, 14);
		}

		[Test]
		public void single_string_q_apos_test() {
			test("\"'str'\"", "'str'", BxlTokenType.String, 1, 1, 7);
		}

		[Test]
		public void single_string_quoted_escaped_test() {
			test("\"\\\"str\\r\\\\\"", "\"str\r\\", BxlTokenType.String, 1, 1, 11);
		}

		[Test]
		public void single_string_quoted_escaped_test_bug_not_escaped_rnt() {
			test("\"\\r\\n\\t\"", "\r\n\t", BxlTokenType.String, 1, 1, 8);
		}

		[Test]
		public void single_string_quoted_test() {
			test("\"str\"", "str", BxlTokenType.String, 1, 1, 5);
		}

		[Test]
		public void single_string_tripple_test() {
			test("\"\"\"str\"\"str\"\"\"", "str\"\"str", BxlTokenType.String, 1, 1, 14);
		}

		[Test]
		public void wspaces_in_strings_not_joined() {
			var tokens = new BxlTokenizer().NoElements.Tokenize("\"1    2\"");
			Assert.AreEqual(3, tokens.Length);
			Assert.AreEqual(BxlTokenType.String, tokens[1].Type);
			Assert.AreEqual("1    2", tokens[1].Value);
		}
	}
}