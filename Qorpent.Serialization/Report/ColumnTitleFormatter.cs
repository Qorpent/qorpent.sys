using System;
using System.Text.RegularExpressions;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Report {
	/// <summary>
	///		Хэлпер для форматирвоания заголовка колонок
	/// </summary>
	public static class ColumnTitleFormatter {
		/// <summary>
		///		Разрешение заголовка
		/// </summary>
		/// <returns>Разрешённый заголовок</returns>
		public static string ResolveTitle(string template, TitleParams titleParams) {
#pragma warning disable 612,618
			if (Regex.IsMatch(template, @"\{\d+\}")) {
				return ResolveOrderedSubstitude(template, titleParams);
			}
#pragma warning restore 612,618
		    return template.Interpolate(titleParams);
		}
		/// <summary>
		///		Разрешение заголовка в старом формате
		/// </summary>
		/// <param name="template"></param>
		/// <param name="titleParams"></param>
		/// <returns></returns>
		[Obsolete("OldZeta compatibility")]
		public static string ResolveOrderedSubstitude(string template, TitleParams titleParams) {
			return string.Format(template,
				titleParams.Year,
				titleParams.Period,
				titleParams.PeriodName,
				titleParams.Year - 1,
				titleParams.Start,
				titleParams.StartPrevMonthLastDay,
				titleParams.Finish,
				titleParams.FinishNextMonthFirstDay
			);
		}
	}
}
