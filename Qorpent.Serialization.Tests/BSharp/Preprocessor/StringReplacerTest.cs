using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Preprocessor;
using Qorpent.Scaffolding.Sql;

namespace Qorpent.Serialization.Tests.BSharp.Preprocessor
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class StringReplacerTest
	{
		[Test]
		public void StrangeBugWithRegex(){
			var sr = new StringReplaceOperation();
			sr.From = "/(ZZindexZZ)+/";
			sr.To = "`{index}";
			var e = XElement.Parse("<param code='report_a_colsetZZindexZZ'>report_a_colsetZZindexZZZZindexZZa</param>");
			sr.Execute(e);
			Assert.AreEqual("<param code=\"report_a_colset`{index}\">report_a_colset`{index}a</param>",e.ToString());
		}
	}
}
