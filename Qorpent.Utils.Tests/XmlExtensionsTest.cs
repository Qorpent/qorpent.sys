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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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
	}
}