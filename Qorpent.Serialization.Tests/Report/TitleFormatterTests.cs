using System;
using NUnit.Framework;
using Qorpent.Report;

namespace Qorpent.Serialization.Tests.Report {
	[TestFixture]
	public class TitleFormatterTests {
		[TestCase(2012, 4, "{2} {0}", "Год", "Год 2012")]
		[TestCase(2013, 1, "{2} {0}", "I кв", "I кв 2013")]
		[TestCase(2013, 2, "{2} {0}", "I пг", "I пг 2013")]
		[TestCase(2013, 3, "{2} {0}", "9 мес", "9 мес 2013")]
		public void IsCorrectTitleResolving(int year, int period, string format, string periodname, string expected) {
			var f = new TitleParams { Year = year, Period = period, PeriodName = periodname };
			Assert.AreEqual(expected, ColumnTitleFormatter.ResolveTitle(format, f));
		}
		[TestCase(2012, 4, "${PeriodName} ${Year}", "Год", "", "Год 2012")]
		[TestCase(2012, 4, "${StartPrevMonthLastDay}", "", "23.02.2013", "31.01.2013")]
		[TestCase(2012, 4, "${p} ${y}", "Год", "", "Год 2012")]
		[TestCase(2012, 4, "${sld}", "", "23.02.2013", "31.01.2013")]
		public void IsCorrectTitleBuildingWithInterPolation(int year, int period, string format, string periodname, string start, string expected) {
			var f = new TitleParams { Year = year, Period = period, PeriodName = periodname };
			if (!string.IsNullOrWhiteSpace(start)) {
				f.StartDate = DateTime.Parse(start);
			}
			Assert.AreEqual(expected, ColumnTitleFormatter.ResolveTitle(format, f));
		}
	}
}