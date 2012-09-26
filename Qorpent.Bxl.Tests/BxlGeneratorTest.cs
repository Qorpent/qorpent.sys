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
// Original file : BxlGeneratorTest.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class BxlGeneratorTest {
		[Test]
		public void GeneratesNamespacePrefixesTest() {
			Assert.AreEqual(
				@"
x=""html://testns""
x::e1
".Trim(), MyBxl.Convert(
					XElement.Parse("<root xmlns:x='html://testns'><x:e1 /></root>"),
					new BxlGeneratorOptions {NoRootElement = true}).Trim()
				);
		}

		[Test]
		public void GeneratesNamespacePrefixesTestWithRoot() {
			Assert.AreEqual(
				@"
x=testns
x::root
	x::e1
".Trim(), MyBxl.Convert(
					XElement.Parse("<x:root xmlns:x='testns'><x:e1 /></x:root>"),
					new BxlGeneratorOptions {NoRootElement = false}).Trim()
				);
		}

		[Test]
		public void WellKnownNamespaceNotCreatedDirectly() {
			Assert.AreEqual(
				@"
qxi::include
".Trim(), MyBxl.Convert(
					XElement.Parse("<root xmlns:qxi='http://qorpent/xml/include'><qxi:include /></root>"),
					new BxlGeneratorOptions {NoRootElement = true}).Trim()
				);
		}
	}
}