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
// Original file : StringExtensionsTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class StringExtensionsTest {
		[Test]
		public void CanConcat_By_Default() {
			var result = new object[] {1, null, "2", 3m}.ConcatString(",");
			Assert.AreEqual("1,,2,3", result);
		}

		[Test]
		public void CanConcat_IgnoreEmpties() {
			var result = new object[] {1, null, "2", 3m}.ConcatString(",", empties: false);
			Assert.AreEqual("1,2,3", result);
		}

		[Test]
		public void CanConcat_NullString() {
			var result = new object[] {1, null, "2", 3m}.ConcatString(",", nullstring: "NULL");
			Assert.AreEqual("1,NULL,2,3", result);
		}

		[Test]
		public void CanConcat_TrailDelimiters() {
			var result = new object[] {1, null, "2", 3m}.ConcatString("/", useTrailDelimiters: true);
			Assert.AreEqual("/1//2/3/", result);
		}

		[Test]
		public void CanSplitByDefaultBehaviour() {
			var splitresult = "1,2 ;; 3/ 4 /5".SmartSplit();
			CollectionAssert.AreEquivalent(new[] {"1", "2", "3", "4", "5"}, splitresult);
		}

		[Test]
		public void CanSplit_With_Empties() {
			var splitresult = "1,2 ;; 3/ 4 /5".SmartSplit(empty: true);
			CollectionAssert.AreEquivalent(new[] {"1", "2", "", "3", "4", "5"}, splitresult);
		}

		[Test]
		public void CanSplit_With_No_Trim() {
			var splitresult = "1,2 ;; 3/ 4 /5".SmartSplit(trim: false);
			CollectionAssert.AreEquivalent(new[] {"1", "2 ", " 3", " 4 ", "5"}, splitresult);
		}

		[Test]
		public void CanSplit_With_Suctom_Delimiter() {
			var splitresult = "1~2~3~4~5".SmartSplit(splitters: '~');
			CollectionAssert.AreEquivalent(new[] {"1", "2", "3", "4", "5"}, splitresult);
		}

		[Test(Description = "IsEmpty returns true for empty strings")]
		public void EmptyStringIsEmpty() {
			Assert.True("".IsEmpty());
		}

		[Test(Description = "IsNotEmpty returns false for empty strings")]
		public void EmptyStringIsFalseIsNotEmpty() {
			Assert.False("".IsNotEmpty());
		}

		[Test(Description = "IsNotEmpty returns true for full strings")]
		public void FullStringIsTrueIsNotEmpty() {
			Assert.True(" x ".IsNotEmpty());
		}

		[Test(Description = "IsEmpty returns false for full strings")]
		public void FullsStringIsEmpty() {
			Assert.False(" x ".IsEmpty());
		}

		[Test(Description = "IsEmpty returns true for null strings")]
		public void NullIsEmpty() {
			Assert.True(((string) null).IsEmpty());
		}

		[Test(Description = "IsNotEmpty returns false for null strings")]
		public void NullIsFalseIsNotEmpty() {
			Assert.False(((string) null).IsNotEmpty());
		}

		[Test(Description = "IsEmpty returns true for ws strings")]
		public void WsStringIsEmpty() {
			Assert.True(" \t \r\n".IsEmpty());
		}

		[Test(Description = "IsNotEmpty returns false for ws strings")]
		public void WsStringIsFalseIsNotEmpty() {
			Assert.False(" \t \r\n".IsNotEmpty());
		}



		[TestCase("/A/B/B/C/", "B", true)]
		[TestCase("/A/B/C/","B",true)]
		[TestCase("/A/B/C/", "C", true)]
		[TestCase("/A/B/C/", "A", true)]
		[TestCase("A/B/C/", "A", false)]
		[TestCase("A/B/C/", "B", true)]
		[TestCase("/A/B/C", "C", false)]
		[TestCase("/A/BC/", "B", false)]
		[TestCase("/A/B/C/", "D", false)]
		[TestCase("", "D", false)]
		[TestCase("/A/B/C/", "", false)]
		public void ListContainsSingleStartEndTest(string src, string val, bool result) {
			Assert.AreEqual(result,src.ListContains(val));
		}

		[TestCase("", "A", "/A/")]
		[TestCase("/a/B/c/", "A", "/a/B/c/")]
		[TestCase("/A/B/C/", "A", "/A/B/C/")]
		[TestCase("/A/B/C/", "D", "/A/B/C/D/")]
		[TestCase("/A/B/C/", "", "/A/B/C/")]
		public void ListAppendSingleStartEndTest(string src, string val, string result) {
			Assert.AreEqual(result,src.ListAppend(val));
		}

		[TestCase("/a/b/C/", "c", "/a/b/")]
		[TestCase("/a/b/c/", "B", "/a/c/")]
		[TestCase("/A/B/C/", "B", "/A/C/")]
		[TestCase("/A/", "A", "")]
		[TestCase("", "A", "")]
		[TestCase("/A/B/C/", "A", "/B/C/")]
		[TestCase("/A/B/C/", "", "/A/B/C/")]
		public void ListRemoveSingleStartEndTest(string src, string val, string result)
		{
			Assert.AreEqual(result, src.ListRemove(val));
		}
	}
}