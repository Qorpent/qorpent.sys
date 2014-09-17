using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    /// <summary>
    ///     Парсер строк даты-времени для Web-ресурсов
    /// </summary>
    /// <remarks>Класс построен по принципу прецедента а не системного анализа дат</remarks>
    public class WebDateTimeParser{
	    private static readonly string[] Formats = new[]{
		    "yyyy-MM-ddTHH:mm(:ss)?_TZ_",
			"dd.MM.yyyy HH:mm Мск",
		    "dd MMM yyyy HH:mm:ss GMT",
		    "dd MMM yyyy, HH:mm - ",
		    "yyyy-MM-ddTHH:mm",
		    "dd.MM.yyyy-HH:mm",
		    "HH:mm | D.MM.yyyy",
		    "yyyy-MM-dd HH:mm",
		    "yyyy-MM-dd HH:mm:ss",
		    "yyyyMMdd HH:mm",
		    "yyyyMMdd HH:mm:ss",
		    "yyyy-MM-dd",
		    "yyyyMMddHHmm",
		    "yyyyMMddHHmmss",
		    "D MMM yyyy HH:mm:ss",
		    "D.MM.yyyy,? HH:mm:ss",
		    "D.MM.yyyy,? HH:mm",
		    "D MMM yyyy HH:mm",
		    "HH:mm dd.MM.yyyy",
		    "D.MM.yyyy",
		    "HH:mm:ss D.MM.yy",
		    "HH:mm, D MMM yyyy",
		    "Вчера в HH:mm",
		    "Сегодня в HH:mm",
		    "yyyyMMdd",
		    "HH:mm dd.MM"
	    };

	    private static readonly Regex[] FormatRegexes = null;
		static WebDateTimeParser(){
			var regexes = new List<Regex>();
			foreach (var format in Formats){
				var regex = format.Replace("yyyy", @"(?<y>\d{4})")
					.Replace("MMM", @"(?<mn>\w+)")
					.Replace("MM", @"(?<m>\d{2})")
					.Replace("dd", @"(?<d>\d\d?)")
					.Replace("D", @"(?<d>\d\d?)")
					.Replace("HH", @"(?<h>\d{2})")
					.Replace("mm", @"(?<mm>\d{2})")
					.Replace("ss", @"(?<s>\d{2})")
					.Replace("_TZ_",@"\+(?<tz>\d+)")
					.Replace("|", "\\|")
					.Replace(" ", @"\s+");
				regexes.Add(new Regex(regex,RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture));
			}
			FormatRegexes = regexes.ToArray();
		}
	    /// <summary>
	    ///     Парсинг сроки дата-время произвольного формата, возвращает ЛОКАЛЬНУЮ дату-время
	    /// </summary>
	    /// <remarks>Локальные даты мы посчитали более удобными для типовых сценариев</remarks>
	    /// <param name="dateTime">Исходная строка</param>
	    /// <param name="sourceUrl">(на вырост) - возможность указать из какого источника получена дата</param>
	    /// <param name="locale">Имя локали по умолчанию (по умолчанию будет текущая)</param>
	    /// <param name="timeZone">При не null будет использоваться для перевода даты локальную</param>
	    /// <returns>Распознанная дата-время</returns>
	    public static DateTime Parse(string dateTime, string sourceUrl= null , string locale = null, int? timeZone = null){
	        dateTime = PrepareDateTimeString(dateTime);
		    foreach (var regex in FormatRegexes){
		        var match = regex.Match(dateTime);
		        if (match.Success){
			        if (null != Log){
				        Log.Debug("Used pattern " + regex);
			        }
			        int month = match.Groups["m"].Value.ToInt();
			        if (!string.IsNullOrWhiteSpace(match.Groups["mn"].Value) && 0 == month){
				        var monthName = match.Groups["mn"].Value;
				        month = GetMonthNumber(locale, monthName);
			        }
			        var year = match.Groups["y"].Value.ToInt();
			        bool isuniversal = false;
			        var day = match.Groups["d"].Value.ToInt();
			        var hour = match.Groups["h"].Value.ToInt();
			        var min = match.Groups["mm"].Value.ToInt();
			        var sec = match.Groups["s"].Value.ToInt();
			        var tz = match.Groups["tz"].Value.ToInt();
					if (0 != tz){
						timeZone = tz;
					}
					if (match.Value.Contains("GMT")){
						isuniversal = true;
					}
			        if (dateTime.ToLowerInvariant().Contains("вчера") && year == 0){
				        var yesterday = DateTime.Today.AddDays(-1);
				        year = yesterday.Year;
				        month = yesterday.Month;
				        day = yesterday.Day;
			        }
			        if (dateTime.ToLowerInvariant().Contains("сегодня") && year == 0){
				        var yesterday = DateTime.Today;
				        year = yesterday.Year;
				        month = yesterday.Month;
				        day = yesterday.Day;
			        }
					
					if (year == 0)
					{
						year = DateTime.Today.Year;
					}
					if (month == 0){
						month = DateTime.Today.Month;
					}
					if (day == 0){
						day = DateTime.Today.Day;
					}

					
			        if (null != Log){
				        Log.Debug(string.Format("Matched {0} {1} {2} {3} {4} {5}", year, month, day, hour, min, sec));
			        }
			        try{
				        var dt = new DateTime(year, month, day, hour, min, sec);
				        if (isuniversal){
					        dt = dt.ToLocalTime();
				        }
				        if (match.Value.Contains("Мск")){
					        dt = dt.AddHours(2);
				        }
						if (null != timeZone){
							var offset = timeZone.Value;
							dt = dt.ToUniversalTime().AddHours(offset);
						}
				        return  dt;
			        }
			        catch{
				        break;
			        }
		        }
	        }
		    return GetBySystemDefinedParsing(dateTime, locale);
	    }

	    private static int GetMonthNumber(string locale, string monthName){
		    var parsedMonth = 0;
		    var culture = CultureInfo.InvariantCulture;
		    if (!string.IsNullOrWhiteSpace(locale)){
			    culture = CultureInfo.GetCultureInfo(locale);
		    }
		    try{
			    parsedMonth = DateTime.ParseExact(monthName, "MMM", culture).Month;
		    }
		    catch{
			    var lower = monthName.ToLowerInvariant();
			    if (lower.StartsWith("янв") || lower.StartsWith("jan")){
				    parsedMonth = 1;
			    }
			    else if (lower.StartsWith("фев") || lower.StartsWith("feb")){
				    parsedMonth = 2;
			    }
			    else if (lower.StartsWith("мар") || lower.StartsWith("mar")){
				    parsedMonth = 3;
			    }
			    else if (lower.StartsWith("апр") || lower.StartsWith("apr")){
				    parsedMonth = 4;
			    }
			    else if (lower.StartsWith("ма") || lower.StartsWith("may")){
				    parsedMonth = 5;
			    }
			    else if (lower.StartsWith("июн") || lower.StartsWith("jun")){
				    parsedMonth = 6;
			    }
			    else if (lower.StartsWith("июл") || lower.StartsWith("jul")){
				    parsedMonth = 7;
			    }
			    else if (lower.StartsWith("авг") || lower.StartsWith("aug")){
				    parsedMonth = 8;
			    }
			    else if (lower.StartsWith("сен") || lower.StartsWith("sep")){
				    parsedMonth = 9;
			    }
			    else if (lower.StartsWith("окт") || lower.StartsWith("oct")){
				    parsedMonth = 10;
			    }
			    else if (lower.StartsWith("ноя") || lower.StartsWith("nov")){
				    parsedMonth = 11;
			    }
			    else if (lower.StartsWith("дек") || lower.StartsWith("dec")){
				    parsedMonth = 12;
			    }
		    }
		    return parsedMonth;
	    }

	    private static DateTime GetBySystemDefinedParsing(string dateTime, string locale){
		    DateTime parsedDate;
		    const DateTimeStyles parseOptions = DateTimeStyles.AllowWhiteSpaces;
		    if (!string.IsNullOrWhiteSpace(locale)){
			    if (DateTime.TryParse(dateTime, CultureInfo.GetCultureInfo(locale), parseOptions, out parsedDate)){
				    return parsedDate;
			    }
		    }
		    if (DateTime.TryParse(dateTime, CultureInfo.CurrentCulture, parseOptions, out parsedDate)){
			    return parsedDate;
		    }
		    if (DateTime.TryParse(dateTime, CultureInfo.InvariantCulture, parseOptions, out parsedDate)){
			    return parsedDate;
		    }

		    throw new Exception("datetime was not responsed");
	    }

	    /// <summary>
		/// Подготовка строки к распознаванию даты - страхует от неверного разбиения или слияния тегов при вытяжке из исходного HTML
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
	    private static string PrepareDateTimeString(string dateTime){
		    string dateTime1 = dateTime;
		    dateTime1 = dateTime1.Trim();
		    dateTime1 = Regex.Replace(dateTime1, @"20\s+[1234]\d\s+", "2014 ");
		    dateTime1 = Regex.Replace(dateTime1, @"(20[1234]\d)(\d{2}:)", "$1 $2");
		    return dateTime1;
	    }

	    /// <summary>
		/// Журнал для отладки
		/// </summary>
		public static IUserLog Log { get; set; }
    }
}