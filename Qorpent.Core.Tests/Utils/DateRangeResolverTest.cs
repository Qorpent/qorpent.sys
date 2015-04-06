using System;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests{
	[TestFixture]
	public class DateRangeResolverTest
	{
		readonly DateTime basis = new DateTime(1990,1,1,12,0,0);
		readonly DateTime monday = new DateTime(2014,10,20,12,0,0);
		readonly DateTime theusday = new DateTime(2014, 10, 21, 12, 0, 0);
		readonly DateTime sunday = new DateTime(2014, 10, 19, 12, 0, 0);
		readonly DateTime saturday = new DateTime(2014, 10, 18, 12, 0, 0);
		[Test]
		public void CanGetToday(){
			var range = basis.CalculateRange("today");
			Assert.AreEqual(new DateTime(1990,1,1),range.Start);
			Assert.AreEqual(new DateTime(1990,1,1,23,59,59),range.Finish);
		}
		[Test]
		public void CanGetYesterday()
		{
			var range = basis.CalculateRange("yesterday");
			Assert.AreEqual(new DateTime(1989, 12, 31), range.Start);
			Assert.AreEqual(new DateTime(1989, 12, 31, 23, 59, 59), range.Finish);
		}

		[Test]
		public void CanGetLastWork(){
			var range = theusday.CalculateRange("lastwork");
			Assert.AreEqual(new DateTime(2014, 10, 20), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 21, 23, 59, 59), range.Finish);

			range = saturday.CalculateRange("lastwork");
			Assert.AreEqual(new DateTime(2014, 10, 17), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 18, 23, 59, 59), range.Finish);

			range = sunday.CalculateRange("lastwork");
			Assert.AreEqual(new DateTime(2014, 10, 17), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 19, 23, 59, 59), range.Finish);

			range = monday.CalculateRange("lastwork");
			Assert.AreEqual(new DateTime(2014, 10, 17), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 20, 23, 59, 59), range.Finish);
		}


		[Test]
		public void CanGetThisWeek()
		{
			var range = theusday.CalculateRange("thisweek");
			Assert.AreEqual(new DateTime(2014, 10, 18), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 24, 23, 59, 59), range.Finish);

			range = saturday.CalculateRange("thisweek");
			Assert.AreEqual(new DateTime(2014, 10, 13), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 19, 23, 59, 59), range.Finish);

			range = sunday.CalculateRange("thisweek");
			Assert.AreEqual(new DateTime(2014, 10, 13), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 19, 23, 59, 59), range.Finish);

			range = monday.CalculateRange("thisweek");
			Assert.AreEqual(new DateTime(2014, 10, 18), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 24, 23, 59, 59), range.Finish);
		}

		[Test]	
		public void CanGetLastWeek()
		{
			var range = theusday.CalculateRange("lastweek");
			Assert.AreEqual(new DateTime(2014, 10, 11), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 17, 23, 59, 59), range.Finish);

			range = saturday.CalculateRange("lastweek");
			Assert.AreEqual(new DateTime(2014, 10, 6), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 12, 23, 59, 59), range.Finish);

			range = sunday.CalculateRange("lastweek");
			Assert.AreEqual(new DateTime(2014, 10, 6), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 12, 23, 59, 59), range.Finish);

			range = monday.CalculateRange("lastweek");
			Assert.AreEqual(new DateTime(2014, 10, 11), range.Start);
			Assert.AreEqual(new DateTime(2014, 10, 17, 23, 59, 59), range.Finish);
		}

		[Test]
		public void DefaultOffSet(){
			Assert.AreEqual(basis.CalculateRange("-2h").Start,basis.AddHours(-2));
			Assert.AreEqual(basis.CalculateRange("2h").Finish,basis.AddHours(2));

			Assert.AreEqual(basis.CalculateRange("-2m").Start, basis.AddMonths(-2));
			Assert.AreEqual(basis.CalculateRange("2m").Finish, basis.AddMonths(2));

			Assert.AreEqual(basis.CalculateRange("-2d").Start, basis.AddDays(-2));
			Assert.AreEqual(basis.CalculateRange("2d").Finish, basis.AddDays(2));

			Assert.AreEqual(basis.CalculateRange("-2w").Start, basis.AddDays(-2*7));
			Assert.AreEqual(basis.CalculateRange("2w").Finish, basis.AddDays(2*7));
		}


	}
}