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
// Original file : LexInfoTest.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Dsl;

namespace Qorpent.Core.Tests.Dsl {
	[TestFixture]
	public class LexInfoTest {
		[Test]
		public void CanCreateCopy() {
			var l = new LexInfo("t", 1, 2, 3, 4);
			var l2 = l.Clone();
			Assert.AreNotSame(l, l2);
			Assert.AreEqual("t", l2.File);
			Assert.AreEqual(1, l2.Line);
			Assert.AreEqual(2, l2.Column);
			Assert.AreEqual(3, l2.CharIndex);
			Assert.AreEqual(4, l2.Length);
		}

		[Test]
		public void CanCreateInstance() {
			var l = new LexInfo("t", 1, 2, 3, 4);
			Assert.AreEqual("t", l.File);
			Assert.AreEqual(1, l.Line);
			Assert.AreEqual(2, l.Column);
			Assert.AreEqual(3, l.CharIndex);
			Assert.AreEqual(4, l.Length);
		}

		[Test]
		public void GeneratesValidString() {
			var l = new LexInfo("t", 1, 2, 3, 4);
			Assert.AreEqual(" at t : 1:2", l.ToString());
		}
	}
}