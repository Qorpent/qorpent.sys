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
// Original file : BxlTokenTest.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class BxlTokenTest {
		[Test]
		public void get_next_with_count_test() {
			var t1 = new BxlToken();
			var t2 = new BxlToken();
			var t3 = new BxlToken();
			var t4 = new BxlToken();
			t1.Next = t2;
			t2.Next = t3;
			t3.Next = t4;
			Assert.AreEqual(t2, t1.GetNext(0));
			Assert.AreEqual(t3, t1.GetNext(1));
			Assert.AreEqual(t4, t1.GetNext(2));
			Assert.AreEqual(null, t1.GetNext(3));
		}

		[Test]
		public void get_prev_with_count_test() {
			var t1 = new BxlToken();
			var t2 = new BxlToken();
			var t3 = new BxlToken();
			var t4 = new BxlToken();
			t1.Next = t2;
			t2.Next = t3;
			t3.Next = t4;
			Assert.AreEqual(t3, t4.GetPrev(0));
			Assert.AreEqual(t2, t4.GetPrev(1));
			Assert.AreEqual(t1, t4.GetPrev(2));
			Assert.AreEqual(null, t4.GetNext(3));
		}

		[Test]
		public void next_is_applyed_automatically() {
			var t = new BxlToken();
			var t2 = new BxlToken();
			t.Prev = t2;
			Assert.AreEqual(t, t2.Next);
		}

		[Test]
		public void next_is_detached_automatically() {
			var t = new BxlToken();
			var t2 = new BxlToken();
			var t3 = new BxlToken();
			t.Prev = t2;
			t.Prev = t3;
			Assert.AreEqual(t, t3.Next);
			Assert.Null(t2.Next);
		}

		[Test]
		public void prev_is_applyed_automatically() {
			var t = new BxlToken();
			var t2 = new BxlToken();
			t.Next = t2;
			Assert.AreEqual(t, t2.Prev);
		}

		[Test]
		public void prev_is_detached_automatically() {
			var t = new BxlToken();
			var t2 = new BxlToken();
			var t3 = new BxlToken();
			t.Next = t2;
			t.Next = t3;
			Assert.AreEqual(t, t3.Prev);
			Assert.Null(t2.Prev);
		}

		[Test]
		public void to_string_test() {
			var t1 = new BxlToken {Value = "1"};
			var t2 = new BxlToken {Value = "2"};
			var t3 = new BxlToken {Value = "3"};
			var t4 = new BxlToken {Value = "4"};
			t1.Next = t2;
			t2.Next = t3;
			t3.Next = t4;
			Assert.AreEqual("1234", t1.ToString(3));
			Assert.AreEqual("123", t1.ToString(2));
			Assert.AreEqual("12", t1.ToString(1));
			Assert.AreEqual("1", t1.ToString(0));
		}
	}
}