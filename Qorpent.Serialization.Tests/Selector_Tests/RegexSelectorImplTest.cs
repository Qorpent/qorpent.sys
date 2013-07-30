using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Minerva.Core.Processing.Selector.Implementations;
using NUnit.Framework;

namespace Minerva.Core.Tests.Selector_Tests
{
	/// <summary>
	/// Проверка селектора на основе регекса
	/// </summary>
	[TestFixture]
	public class RegexSelectorImplTest
	{
		private RegexSelectorImpl rx;

		[SetUp]
		public void Setup() {
			rx = new RegexSelectorImpl();
		}

		/// <summary>
		/// Унифицированный метод выполнения теста
		/// </summary>
		/// <param name="srcxml"></param>
		public string Execute(XElement root,string query) {
			var result = rx.Select(root, query);
			return string.Join(";", result.Select(_ => _.ToString().Replace("\"","'")));
		}

		public const string DEFAULT_TEST_XML = "<a><b i='1' id='xyy'>xxx</b><b>yyy</b><b i='2'>xyy</b><c>xxx</c></a>";

		[TestCase(DEFAULT_TEST_XML, "[^y]y+##++-", "<b i='1' id='xyy'>xxx</b>;<b i='2'>xyy</b>")]
		[TestCase(DEFAULT_TEST_XML, "[^y]y+##-+-", "<b i='1' id='xyy'>xxx</b>")]
		[TestCase(DEFAULT_TEST_XML, "[^y]y+", "<b i='2'>xyy</b>")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--!b,c", "")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--b,c", "<b i='1' id='xyy'>xxx</b>;<c>xxx</c>")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--b", "<b i='1' id='xyy'>xxx</b>")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--!c", "<b i='1' id='xyy'>xxx</b>")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--!b", "<c>xxx</c>")]
		[TestCase(DEFAULT_TEST_XML, "xxx##+--c", "<c>xxx</c>")]
		
		public void MainFeaturedTest(string srcxml,string query, string outxml) {
			var xml = XElement.Parse(srcxml);
			var result = Execute(xml, query);
			Assert.AreEqual(outxml,result);
		}
	}
}
