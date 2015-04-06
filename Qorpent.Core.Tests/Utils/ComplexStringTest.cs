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
// Original file : MainTest.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Utils;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.Dsl.Tests {
	[TestFixture]
	public class MainTest {
		private void test(bool expectedresult, string options, string expression) {
			var optionsparser = ComplexStringHelper.AutoDetect(options);
			//optionsparser.EmptyValue = "1";
			var toptions = optionsparser.Parse(options);
			var termsrc = LogicTermSource.Create(toptions);
			var expr = new LogicalExpressionParser().Parse(expression);
			var result = expr.Eval(termsrc);
			Assert.AreEqual(expectedresult, result);
		}

		[Test]
		public void blocks_false() {
			test(false, "A:3|B:4|C:3", "( A & D ) | ( C & X )");
		}

		[Test]
		public void blocks_true() {
			test(true, "A:3|B:4|C:3", "( A | D ) & ( C | X )");
		}

		[Test]
		public void conjunction_and_not_false() {
			test(false, "A|B", "A & !B");
		}

		[Test]
		public void conjunction_and_not_true() {
			test(true, "A|C", "A & !B");
		}

		[Test]
		public void conjunction_and_not_true_2() {
			test(true, "D|C", "!A & !B");
		}


		[Test]
		public void conjunction_false() {
			test(false, "A|C", "A&B");
		}

		[Test]
		public void conjunction_false_ignore_ws() {
			test(false, "A|C", "A & B");
		}

		[Test]
		public void conjunction_true() {
			test(true, "A|B", "A&B");
		}

		[Test]
		public void conjunction_true_ignore_ws() {
			test(true, "A|B", "A & B");
		}

		[Test]
		public void disjunction_false() {
			test(false, "D|C", "A | B");
		}

		[Test]
		public void disjunction_true() {
			test(true, "A|C", "A | B");
		}

		[Test]
		public void disjunction_true_2() {
			test(true, "B|C", "A | B");
		}

		[Test]
		public void equal_1_false() {
			test(false, "A|C", "A = B");
		}

		[Test]
		public void equal_1_true() {
			test(true, "A|B", "A = B");
		}


		[Test]
		public void equal_2_true() {
			test(true, "C|D", "A = B");
		}

		[Test]
		public void long_conjunct_true() {
			test(true, "A:3|B:4|C:3", "A & B & C & !D & $A = $C & $B != $C");
		}

		[Test]
		public void single_literal_false() {
			test(false, "B", "A");
		}

		[Test]
		public void single_literal_false_neg() {
			test(true, "B", "!A");
		}

		[Test]
		public void single_literal_true() {
			test(true, "A", "A");
		}

		[Test]
		public void single_literal_true_neg() {
			test(false, "A", "!A");
		}

		[Test]
		public void veq_1_false() {
			test(false, "A:3", "A = '4'");
		}

		[Test]
		public void veq_1_neqfalse() {
			test(true, "A:3", "A != '4'");
		}

		[Test]
		public void veq_1_neqtrue() {
			test(false, "A:3", "A != '3'");
		}

		[Test]
		public void veq_1_true() {
			test(true, "A:3", "A = '3'");
		}

		[Test]
		public void veq_var_false() {
			test(false, "A:3|B:4", "$A = $B");
		}

		[Test]
		public void veq_var_true() {
			test(true, "A:3|B:3", "$A = $B");
		}
	}
}