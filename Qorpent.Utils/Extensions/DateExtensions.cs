using System;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// Вспомогательные функции для дат
	/// </summary>
	public static class DateExtensions {

		/// <summary>
		/// accomodates dates before Begin to given year with offset
		/// </summary>
		/// <param name="source">source date</param>
		/// <param name="year">base year</param>
		/// <returns>accommodated year</returns>
		/// <remarks>usefull for economical applications, we
		/// can define and keep in database date in such form:
		/// 1899-01-01, that means "first jan of given year", because
		/// after  new DateTime(1899,1,1).accomodateToYear(2009) =&gt; 2009-01-01"</remarks>
		public static DateTime AccomodateToYear(this DateTime source, int year)
		{
			if (source.Year >= QorpentConst.Date.Begin.Year) return source;
			var newYear = year - (QorpentConst.Date.Begin.Year - 1 - source.Year);
			var result = new DateTime(newYear, source.Month, source.Day, source.Hour, source.Minute, source.Second);
			if (result.Month == 2 && result.Day == 28 && (result.Year % 4 == 0))
			{
				result = result.AddDays(1);
			}
			return result;
		}

		/// <summary>
		/// Checks if datetime is logical null 
		/// </summary>
		/// <param name="date">date to check</param>
		/// <returns>true if date &lt;= Begin or date &gt;= End</returns>
		public static bool IsDateNull(this DateTime date)
		{
			return date <= QorpentConst.Date.Begin || date >= QorpentConst.Date.End;
		}

	}
}