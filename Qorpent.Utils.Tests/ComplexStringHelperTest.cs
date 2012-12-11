using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Utils.Tests
{
	[TestFixture]
	public class ComplexStringHelperTest
	{
		[TestCase("/`a:1~~`z/ ", "/a", "1::/z")]
		[TestCase("/a:1/ /b/ /c:/ /dx:xxx/","a","1","b","","c","","dx","xxx")]
		public void TagParserTest(string s, params string[] tests) {
			var dict = ComplexStringHelper.CreateTagParser().Parse(s);
			for(int i=0;i<tests.Count()-1;i=i+2) {
				var n = tests[i];
				var v = tests[i + 1];
				Assert.AreEqual(dict[n],v);
			}
			
		}


		[TestCase("`a:1~~`z|b", "|a", "1::|z","b","")]
		[TestCase("a:1|b|c|dx:xxx", "a", "1", "b", "", "c", "", "dx", "xxx")]
		public void CsParserTest(string s, params string[] tests)
		{
			var dict = ComplexStringHelper.CreateComplexStringParser().Parse(s);
			for (int i = 0; i < tests.Count() - 1; i = i + 2)
			{
				var n = tests[i];
				var v = tests[i + 1];
				Assert.AreEqual(dict[n], v);
			}
		}

		[TestCase("`a:1~~`z b", " a", "1:: z", "b", "")]
		[TestCase("a:1 b c dx:xxx", "a", "1", "b", "", "c", "", "dx", "xxx")]
		public void WsCsParserTest(string s, params string[] tests)
		{
			var dict = ComplexStringHelper.CreateWSComplexStringParser().Parse(s);
			for (int i = 0; i < tests.Count() - 1; i = i + 2)
			{
				var n = tests[i];
				var v = tests[i + 1];
				Assert.AreEqual(dict[n], v);
			}
		}


		[TestCase("`a:1~~`z b", " a", "1:: z", "b", "")]
		[TestCase("a:1 b c dx:xxx", "a", "1", "b", "", "c", "", "dx", "xxx")]
		[TestCase("`a:1~~`z|b", "|a", "1::|z")]
		[TestCase("a:1|b|c|dx:xxx", "a", "1", "b", "", "c", "", "dx", "xxx")]
		[TestCase("/`a:1~~`z/ ", "/a", "1::/z")]
		[TestCase("/a:1/ /b/ /c:/ /dx:xxx/", "a", "1", "b", "", "c", "", "dx", "xxx")]
		public void AutoDetectParserTest(string s, params string[] tests)
		{
			var dict = ComplexStringHelper.AutoDetect(s).Parse(s);
			for (int i = 0; i < tests.Count() - 1; i = i + 2)
			{
				var n = tests[i];
				var v = tests[i + 1];
				Assert.AreEqual(dict[n], v);
			}

		}
	}
}
