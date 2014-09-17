using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    /// <summary>
    ///     Парсер строк даты-времени спонтанного формата
    /// </summary>
    public class DateTimeParser {
        /// <summary>
        ///     Безопасный парсинг строки дата-время произвольного формата
        /// </summary>
        /// <param name="dateTime">Исходная строка</param>
        /// <param name="safeReturn">Safe-back значение, котоое будет возвращено в случае сбоя распозначания</param>
        /// <returns>Распознанная дата-время</returns>
        public DateTime TryParse(string dateTime,DateTime safeReturn = default(DateTime)) {
			
            try {
                return Parse(dateTime);
            } catch{
	            return safeReturn;

            }
        }
        /// <summary>
        ///     Парсинг сроки дата-время произвольного формата
        /// </summary>
        /// <param name="dateTime">Исходная строка</param>
        /// <returns>Распознанная дата-время</returns>
        public DateTime Parse(string dateTime){
	        string dateTime1 = dateTime;
	        dateTime1 = dateTime1.Trim();
	        dateTime1 = Regex.Replace(dateTime1, @"20\s+[1234]\d\s+", "2014 ");
	        dateTime1 = Regex.Replace(dateTime1, @"(20[1234]\d)(\d{2}:)", "$1 $2");
	        foreach (var sdf in new[]{
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
	        }){
		        var regex = sdf.Replace("yyyy", @"(?<y>\d{4})").Replace("MMM", @"(?<mn>\w+)").Replace("MM", @"(?<m>\d{2})").Replace("dd", @"(?<d>\d\d?)").Replace("D", @"(?<d>\d\d?)").Replace("HH", @"(?<h>\d{2})").Replace("mm", @"(?<mm>\d{2})").Replace("ss", @"(?<s>\d{2})").Replace("|", "\\|").Replace(" ", @"\s+");

		        var match = Regex.Match(dateTime1, regex, RegexOptions.Compiled|RegexOptions.IgnoreCase);
		        if (match.Success){
			        if (match.Value.Contains("T")) break;
			        if (null != Log){
				        Log.Debug("Used pattern " + sdf);
			        }
			        var culture = CultureInfo.InvariantCulture;
			        int month = match.Groups["m"].Value.ToInt();
			        if (!string.IsNullOrWhiteSpace(match.Groups["mn"].Value) && 0 == month){
				        var monthName = match.Groups["mn"].Value;
				        if (-1 == monthName.ToLowerInvariant().IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray())){
					        culture = CultureInfo.GetCultureInfo("RU-ru");
				        }
				        try{
					        month = DateTime.ParseExact(monthName, "MMM", culture).Month;
				        }
				        catch{
					        var lower = monthName.ToLowerInvariant();
					        if (lower.StartsWith("янв") || lower.StartsWith("jan")){
						        month = 1;
					        }
					        else if (lower.StartsWith("фев") || lower.StartsWith("feb")){
						        month = 2;
					        }
					        else if (lower.StartsWith("мар") || lower.StartsWith("mar")){
						        month = 3;
					        }
					        else if (lower.StartsWith("апр") || lower.StartsWith("apr")){
						        month = 4;
					        }
					        else if (lower.StartsWith("ма") || lower.StartsWith("may")){
						        month = 5;
					        }
					        else if (lower.StartsWith("июн") || lower.StartsWith("jun")){
						        month = 6;
					        }
					        else if (lower.StartsWith("июл") || lower.StartsWith("jul")){
						        month = 7;
					        }
					        else if (lower.StartsWith("авг") || lower.StartsWith("aug")){
						        month = 8;
					        }
					        else if (lower.StartsWith("сен") || lower.StartsWith("sep")){
						        month = 9;
					        }
					        else if (lower.StartsWith("окт") || lower.StartsWith("oct")){
						        month = 10;
					        }
					        else if (lower.StartsWith("ноя") || lower.StartsWith("nov")){
						        month = 11;
					        }
					        else if (lower.StartsWith("дек") || lower.StartsWith("dec")){
						        month = 12;
					        }
				        }
			        }
			        var year = match.Groups["y"].Value.ToInt();
					
			        var day = match.Groups["d"].Value.ToInt();
			        var hour = match.Groups["h"].Value.ToInt();
			        var min = match.Groups["mm"].Value.ToInt();
			        var sec = match.Groups["s"].Value.ToInt();
			        if (dateTime1.ToLowerInvariant().Contains("вчера") && year == 0){
				        var yesterday = DateTime.Today.AddDays(-1);
				        year = yesterday.Year;
				        month = yesterday.Month;
				        day = yesterday.Day;
			        }
			        if (dateTime1.ToLowerInvariant().Contains("сегодня") && year == 0){
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
				        if (match.Value.Contains("GMT")){
					        dt = dt.ToLocalTime();
				        }
				        if (match.Value.Contains("Мск")){
					        dt = dt.AddHours(2);
				        }
				        return  dt;
			        }
			        catch{
				        break;
			        }
		        }
	        }

	        try{
		        return DateTime.Parse(dateTime1);
	        }
	        catch (Exception){
	        }
	        try{
		        return DateTime.Parse(dateTime1, CultureInfo.GetCultureInfo("RU-ru"));
	        }
	        catch (Exception){
	        }
	        if (null != Log){
		        Log.Debug("Old style parsing used");
	        }

	        throw new Exception("not more supported");
        }

	    /// <summary>
		/// Журнал для отладки
		/// </summary>
		public static IUserLog Log { get; set; }
    }
}