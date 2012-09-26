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
// Original file : ByLineParsingTest.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using NUnit.Framework;

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class ByLineParsingTest {
		private BxlTokenizer tokenizer;

		private BxlToken[] tokens(string code, bool opened = false, bool attr = false, bool str = false, int nest = 1) {
			tokenizer = new BxlTokenizer {AllowNonClosedStringsAndExpressions = true};
			if (opened) {
				if (attr) {
					tokenizer.HasOpenedAttributeAtStart = true;
				}
				else {
					tokenizer.HasOpenedValueAtStart = true;
				}
				if (str) {
					tokenizer.HasOpenedStringAtStart = true;
				}
				else {
					tokenizer.HasOpenedExpressionAtStart = true;
					tokenizer.ExpressionNestLevelAtStart = nest;
				}
			}
			return tokenizer.Tokenize(code);
		}

		[Test]
		public void can_process_addition_items_after_attribute() {
			var ts = tokens("dddd\"\"\" x=2 : ddd", true, true, true);
			var str = TokenizerTestHelper.GetShortTypedNotation(ts, "_");
			Console.WriteLine(str);
			Assert.AreEqual("{_AAV}", str);
			Assert.AreEqual(1, tokenizer.GetFinishState());
		}


		[Test]
		public void can_setup_state_from_code() {
			var t = new BxlTokenizer();
			t.SetInitialState(1);
			Assert.True(t.AllowNonClosedStringsAndExpressions);
			Assert.AreEqual(0, t.ExpressionNestLevelAtStart);
			t.SetInitialState(11000);
			Assert.True(t.HasOpenedValueAtStart);
			Assert.True(t.HasOpenedStringAtStart);
			Assert.Throws<BxlException>(() => t.SetInitialState(12000));
			t.SetInitialState(12003);
			Assert.True(t.HasOpenedValueAtStart);
			Assert.True(t.HasOpenedExpressionAtStart);
			Assert.AreEqual(3, t.ExpressionNestLevelAtStart);
			t.SetInitialState(21000);
			Assert.True(t.HasOpenedAttributeAtStart);
			var exc = Assert.Throws<BxlException>(() => t.SetInitialState(20000));
			StringAssert.Contains("string", exc.Message);
		}

		[Test]
		public void cannot_process_addition_items_after_value() {
			Assert.Throws<BxlException>(() => tokens("dddd\"\"\" x=2", true, false, true));
		}

		[Test]
		public void expression_end_and_attribute_end() {
			var t = tokens("expression end) ", true, true, false).Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeEnd, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionEnd, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(15, t.LexInfo.Length);
			Assert.True(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, tokenizer.GetFinishState());
		}

		[Test]
		public void expression_end_and_value_end() {
			var t = tokens("expression end) ", true, false, false).Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValueEnd, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionEnd, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(15, t.LexInfo.Length);
			Assert.True(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, tokenizer.GetFinishState());
		}

		[Test]
		public void expression_part_in_attribute_part() {
			var t = tokens("expression ( part (", true, true, false).Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributePart, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionPart, t.SubType);
			Assert.AreEqual(3, t.NestLevel);
			Assert.False(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(19, t.LexInfo.Length);
			Assert.AreEqual(22003, tokenizer.GetFinishState());
		}

		[Test]
		public void expression_part_in_value_part() {
			var t = tokens("expression ( part (", true, false, false).Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValuePart, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionPart, t.SubType);
			Assert.AreEqual(3, t.NestLevel);
			Assert.False(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(19, t.LexInfo.Length);
			Assert.AreEqual(12003, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_anonym_attribute_with_expression() {
			var t = tokens("e (start (x").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeStart, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionStart, t.SubType);
			Assert.AreEqual("(start (x", t.Value);
			Assert.AreEqual(2, t.NestLevel);
			Assert.True(tokenizer.HasOpenedAttributeAtEnd);
			Assert.True(tokenizer.HasOpenedExpressionAtEnd);
			Assert.AreEqual(2, tokenizer.ExpressionNestLevelAtEnd);
			Assert.AreEqual(3, t.LexInfo.Column);
			Assert.AreEqual(9, t.LexInfo.Length);
			Assert.AreEqual(22002, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_anonym_attribute_with_tripple_string() {
			var t = tokens("e \"\"\"start").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeStart, t.Type);
			Assert.AreEqual(BxlTokenType.StringStart, t.SubType);
			Assert.AreEqual("start", t.Value);
			Assert.True(tokenizer.HasOpenedAttributeAtEnd);
			Assert.True(tokenizer.HasOpenedStringAtEnd);
			Assert.AreEqual(3, t.LexInfo.Column);
			Assert.AreEqual(8, t.LexInfo.Length);

			Assert.AreEqual(21000, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_element_value_with_expression() {
			var t = tokens("e : (start (x").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValueStart, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionStart, t.Impl.Type);
			Assert.AreEqual("(start (x", t.Value);
			Assert.AreEqual(2, t.Impl.NestLevel);
			Assert.True(tokenizer.HasOpenedValueAtEnd);
			Assert.True(tokenizer.HasOpenedExpressionAtEnd);
			Assert.AreEqual(2, tokenizer.ExpressionNestLevelAtEnd);
			Assert.AreEqual(5, t.LexInfo.Column);
			Assert.AreEqual(9, t.LexInfo.Length);
			Assert.AreEqual(12002, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_element_value_with_tripple_string() {
			var t = tokens("e : \"\"\"start").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValueStart, t.Type);
			Assert.AreEqual(BxlTokenType.StringStart, t.Impl.Type);
			Assert.AreEqual("start", t.Value);
			Assert.True(tokenizer.HasOpenedValueAtEnd);
			Assert.True(tokenizer.HasOpenedStringAtEnd);
			Assert.AreEqual(5, t.LexInfo.Column);
			Assert.AreEqual(8, t.LexInfo.Length);

			Assert.AreEqual(11000, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_named_attribute_with_expression() {
			var t = tokens("e x=(start (x").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeStart, t.Type);
			Assert.AreEqual(BxlTokenType.ExpressionStart, t.Right.Type);
			Assert.AreEqual("(start (x", t.Right.Value);
			Assert.AreEqual(2, t.Right.NestLevel);
			Assert.True(tokenizer.HasOpenedAttributeAtEnd);
			Assert.True(tokenizer.HasOpenedExpressionAtEnd);
			Assert.AreEqual(2, tokenizer.ExpressionNestLevelAtEnd);
			Assert.AreEqual(5, t.Right.LexInfo.Column);
			Assert.AreEqual(9, t.Right.LexInfo.Length);
			Assert.AreEqual(22002, tokenizer.GetFinishState());
		}

		[Test]
		public void generates_start_of_named_attribute_with_tripple_string() {
			var t = tokens("e x=\"\"\"start").Reverse().Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeStart, t.Type);
			Assert.AreEqual(BxlTokenType.StringStart, t.Right.Type);
			Assert.AreEqual("start", t.Right.Value);
			Assert.True(tokenizer.HasOpenedAttributeAtEnd);
			Assert.True(tokenizer.HasOpenedStringAtEnd);
			Assert.AreEqual(5, t.Right.LexInfo.Column);
			Assert.AreEqual(8, t.Right.LexInfo.Length);

			Assert.AreEqual(21000, tokenizer.GetFinishState());
		}

		[Test]
		public void single_line_still_exception() {
			Assert.Throws<BxlException>(() => tokens("e : \"start"));
		}

		[Test]
		public void string_end_and_attribute_end() {
			var t = tokens("string end\"\"\"", true, true, true).Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributeEnd, t.Type);
			Assert.AreEqual(BxlTokenType.StringEnd, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(13, t.LexInfo.Length);
			Assert.True(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, tokenizer.GetFinishState());
		}


		[Test]
		public void string_end_and_value_end() {
			var t = tokens("string end\"\"\"", true, false, true).Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValueEnd, t.Type);
			Assert.AreEqual(BxlTokenType.StringEnd, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(13, t.LexInfo.Length);
			Assert.True(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(1, tokenizer.GetFinishState());
		}

		[Test]
		public void string_part_in_attribute_part() {
			var t = tokens("string part", true, true, true).Skip(1).First();
			Assert.AreEqual(BxlTokenType.AttributePart, t.Type);
			Assert.AreEqual(BxlTokenType.StringPart, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(11, t.LexInfo.Length);
			Assert.False(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(21000, tokenizer.GetFinishState());
		}

		[Test]
		public void string_part_in_value_part() {
			var t = tokens("string part", true, false, true).Skip(1).First();
			Assert.AreEqual(BxlTokenType.ValuePart, t.Type);
			Assert.AreEqual(BxlTokenType.StringPart, t.SubType);
			Assert.AreEqual(1, t.LexInfo.Column);
			Assert.AreEqual(11, t.LexInfo.Length);
			Assert.False(tokenizer.OpenedStartIsClosed);
			Assert.AreEqual(11000, tokenizer.GetFinishState());
		}
	}
}