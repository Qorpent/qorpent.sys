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
// Original file : XmlExtensionsTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class XmlExtensionsTest {
		private static XmlDocument getdoc() {
			var result = new XmlDocument();
			result.LoadXml("<hello />");
			return result;
		}

		private static readonly object[] XmlSources = new object[]
			{
				XElement.Parse("<hello />"),
				"<hello />",
				new StringReader("<hello />"),
				getdoc(),
				getdoc().CreateNavigator(),
				XElement.Parse("<hello />").CreateReader()
			};

		private class ArrayItem {
			public string Name;
		}

		
		[Test, TestCaseSource("XmlSources")]
		public void XmlFromAnyTest(object src) {
			var result = XmlExtensions.GetXmlFromAny(src);
			Assert.AreEqual("<hello />", result.ToString());
		}


		[Test]
		public void CanGetValidXPath(){
			var xml = XElement.Parse(@"<a>
<p/>
<p/>
<b a='1'/>
<b a='2'>
	<m a='3'/>
	<n/>
	<m/>
</b>
<a/>
<a/>
</a>");
			foreach (var element in xml.Descendants()){
				
				var xpath = element.GetXPath();
				Console.WriteLine(xpath);
				Assert.AreEqual(element,xml.XPathSelectElement(xpath));
				var elements = xml.XPathSelectElements(xpath);
				Assert.AreEqual(1,elements.Count());

				foreach (var a in element.Attributes()){
					xpath = a.GetXPath();
					Console.WriteLine(xpath);
					Assert.AreEqual(a.Parent.GetXPath()+"/@"+a.Name.LocalName,xpath);
				}
			}
		}

		public class Serj {
			public int Dno { get; set; }
		}
		[Test]
		public void IsCorrectApplyMapping() {
			const string xml = @"<r dnishe=""1"" />";
			var serj = XElement.Parse(xml).Apply<Serj>(map:new {dnishe = "Dno"});
			Assert.AreEqual(1, serj.Dno);
		}
		[Test]
		public void IsCorrectApplyMappingWithNotExistingAttributesInSource() {
			const string xml = @"<r serj_dnishe=""1"" />";
			var serj = XElement.Parse(xml).Apply<Serj>(map: new { dnishe = "Dno" });
			Assert.AreEqual(0, serj.Dno);
		}

        [Test]
        public void IdsAreApplyed() {
            var src = XElement.Parse(@"
<a>
<b>
    <d/>
    <f/>
</b>
<c>
    <g/>
    <h/>
</c>
</a>
");
            var result = src.GenerateUniqueIdsForElements("id");
            Console.WriteLine(result.ToString());
            Console.WriteLine("============================================================");
            string escaped = result.ToString().Replace("\"", "\"\"");
            Console.WriteLine(escaped);

            Assert.AreEqual(@"<a id=""0"">
  <b id=""1"">
    <d id=""2"" />
    <f id=""3"" />
  </b>
  <c id=""4"">
    <g id=""5"" />
    <h id=""6"" />
  </c>
</a>", result.ToString());
        }
	}
}