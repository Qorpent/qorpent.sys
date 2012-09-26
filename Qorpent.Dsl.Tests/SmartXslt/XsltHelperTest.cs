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
// Original file : XsltHelperTest.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Dsl.SmartXslt;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl.Tests.SmartXslt {
	[TestFixture]
	public class XsltHelperTest {
		[SetUp]
		public void Setup() {
			helper = new XsltHelper();
			basexslt = XElement.Parse(basexslt_);
		}


		private XsltHelper helper;
		private string basexslt_ = @"
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'
    xmlns:msxsl='urn:schemas-microsoft-com:xslt' exclude-result-prefixes='msxsl'
>
    <xsl:output method='xml' indent='yes'/>
	<xsl:template match='@*|node()'>
		<xsl:copy>
			<xsl:apply-templates select='@*|node()'/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
";
		private XElement basexslt;

		private class myext {}

		[Test]
		public void IncludeExtensionNoDuplicate_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt,
			                                        new[]
				                                        {
					                                        XsltExtensionDefinition.Extension(new myext()),
					                                        XsltExtensionDefinition.Extension(new myext())
				                                        });
			Console.WriteLine(basexslt);
			var e = basexslt.Attribute("{" + XNamespace.Xmlns + "}myext");
			Assert.NotNull(e);
			Assert.AreEqual(typeof (myext).FullName, e.Value);
		}

		[Test]
		public void IncludeExtensionPreventOverride_ToXslt() {
			Assert.Throws<QorpentException>(() => helper.PrepareXsltStylesheet(basexslt,
			                                                                   new[]
				                                                                   {
					                                                                   XsltExtensionDefinition.Extension(new myext()),
					                                                                   XsltExtensionDefinition.Extension(new myext(),
					                                                                                                     ns: "custom")
				                                                                   }))
				;
		}

		[Test]
		public void IncludeExtension_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[] {XsltExtensionDefinition.Extension(new myext())});
			Console.WriteLine(basexslt);
			var e = basexslt.Attribute("{" + XNamespace.Xmlns + "}myext");
			Assert.NotNull(e);
			Assert.AreEqual(typeof (myext).FullName, e.Value);
		}

		[Test]
		public void IncludeImport_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[] {XsltExtensionDefinition.Import("href1")});
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.ImportElementName, e.Name.ToString());
			Assert.AreEqual("href1", e.Attr("href"));
		}

		[Test]
		public void IncludeInclude_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[] {XsltExtensionDefinition.Include("href1")});
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.IncludeElementName, e.Name.ToString());
			Assert.AreEqual("href1", e.Attr("href"));
		}

		[Test]
		public void IncludeParameterSelect_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[] {XsltExtensionDefinition.ParameterSelect("n", "//v")});
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.ParamElementName, e.Name.ToString());
			Assert.AreEqual("n", e.Attr("name"));
			Assert.AreEqual("//v", e.Attr("select"));
		}

		[Test]
		public void IncludeParameter_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[] {XsltExtensionDefinition.Parameter("n", "v")});
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.ParamElementName, e.Name.ToString());
			Assert.AreEqual("n", e.Attr("name"));
			Assert.AreEqual("v", e.Value);
		}


		[Test]
		public void ParameterOverride_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt, new[]
				{
					XsltExtensionDefinition.Parameter("n", "v"),
					XsltExtensionDefinition.Parameter("n", "v2")
				});
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.ParamElementName, e.Name.ToString());
			Assert.AreEqual("n", e.Attr("name"));
			Assert.AreEqual("v2", e.Value);
		}

		[Test]
		public void ParameterSelectOverride_ToXslt() {
			basexslt = helper.PrepareXsltStylesheet(basexslt,
			                                        new[]
				                                        {
					                                        XsltExtensionDefinition.Parameter("n", "v"),
					                                        XsltExtensionDefinition.ParameterSelect("n", "//v")
				                                        });
			Console.WriteLine(basexslt);
			var e = basexslt.Elements().First();
			Assert.AreEqual(XsltHelper.ParamElementName, e.Name.ToString());
			Assert.AreEqual("n", e.Attr("name"));
			Assert.AreEqual("//v", e.Attr("select"));
		}


		[Test]
		public void ValidExtensionsOrder() {
			basexslt = helper.PrepareXsltStylesheet(basexslt,
			                                        new[]
				                                        {
					                                        XsltExtensionDefinition.Parameter("x"),
					                                        XsltExtensionDefinition.Include("y"),
					                                        XsltExtensionDefinition.Import("z"),
					                                        XsltExtensionDefinition.Parameter("a"),
					                                        XsltExtensionDefinition.Include("b"),
					                                        XsltExtensionDefinition.Import("c"),
				                                        });
			Console.WriteLine(basexslt);
			var im1 = basexslt.Elements().ElementAt(0);
			var im2 = basexslt.Elements().ElementAt(1);
			var in1 = basexslt.Elements().ElementAt(2);
			var in2 = basexslt.Elements().ElementAt(3);
			var p1 = basexslt.Elements().ElementAt(4);
			var p2 = basexslt.Elements().ElementAt(5);

			Assert.AreEqual(XsltHelper.ImportElementName, im1.Name.ToString());
			Assert.AreEqual(XsltHelper.ImportElementName, im2.Name.ToString());
			Assert.AreEqual(XsltHelper.IncludeElementName, in1.Name.ToString());
			Assert.AreEqual(XsltHelper.IncludeElementName, in2.Name.ToString());
			Assert.AreEqual(XsltHelper.ParamElementName, p1.Name.ToString());
			Assert.AreEqual(XsltHelper.ParamElementName, p2.Name.ToString());

			Assert.AreEqual("z", im1.Attr("href"));
			Assert.AreEqual("c", im2.Attr("href"));
			Assert.AreEqual("y", in1.Attr("href"));
			Assert.AreEqual("b", in2.Attr("href"));
			Assert.AreEqual("x", p1.Attr("name"));
			Assert.AreEqual("a", p2.Attr("name"));
		}
	}
}