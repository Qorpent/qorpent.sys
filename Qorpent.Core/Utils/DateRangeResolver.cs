using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	///		Резольвер промежутка даты/времени
	/// </summary>
	public static  class DateRangeResolver {
        static readonly DateRange Any = new DateRange{Start = QorpentConst.Date.Begin,Finish = QorpentConst.Date.End};
		///  <summary>
		/// 		Разрешение промежутка даты/времени
		///  </summary>
		/// <param name="baseDate"></param>
		/// <param name="expression">Выражение для разрешения</param>
		/// <param name="holidays"></param>
		/// <returns>Разрешённая дата/время</returns>
		public static DateRange CalculateRange(this DateTime baseDate, string expression, params DateTime[] holidays){
		    if (expression == "any") {
		        return Any;
		    }
			expression = (expression ?? "").Trim().ToLowerInvariant();
			var result = new DateRange{Base = baseDate, Expression = expression};
			//сначала разбираем стандартные периоды
			if (expression == "today"||expression=="сегодня"){
				GetToDay(baseDate, result);
			}else if (expression == "yesterday"||expression=="вчера"){
				GetYesterDay(baseDate, result);
			}else if (expression == "lastwork"||expression=="вработе"){
				GetLastWork(baseDate, holidays, result);
			}else if (expression == "thisweek" || expression == "этанеделя"){
				//суббота и воскресенье относятся к ТЕКУЩЕЙ недели, так как просмотр данных за выходные обычно приходится на понедельник
				//но при этом если текущая база приходится на субботу или воскресенье, то они относятся к предыдущей неделе
				GetThisWeek(baseDate, result);
			}
			else if (expression == "lastweek" || expression == "преднеделя")
			{
				GetLastWeek(baseDate, result);
			}
			else{
				var match = Regex.Match(expression, @"^(-?\d+)(\w+)$");
				var type = match.Groups[2].Value;
				var offset = match.Groups[1].Value.ToInt();
				GetOffseted(baseDate,type, offset,result);
			}
			return result;
		}

		private static void GetLastWeek(DateTime baseDate, DateRange result){
			GetThisWeek(baseDate,result);
			result.Start = result.Start.AddDays(-7);
			result.Finish = result.Finish.AddDays(-7);
		}

		private static void GetThisWeek(DateTime baseDate, DateRange result){
			var offset = baseDate.Date.DayOfWeek.ToInt();
			if (baseDate.DayOfWeek == DayOfWeek.Saturday || baseDate.DayOfWeek == DayOfWeek.Sunday){
				if (offset == 0){
					offset = 6;
				}
				else{
					offset = 5;
				}
			}
			else{
				offset += 1;
				if (offset == 7) offset = 0;	
			}
			
			var start = baseDate.Date.AddDays(-offset);
			var finish = start.AddDays(7).AddSeconds(-1);
			result.Start = start;
			result.Finish = finish;
		}

		private static void GetOffseted(DateTime baseDate, string type, int offset, DateRange result){
			var anotherDate = baseDate;
			if (type == "h"){
				anotherDate = baseDate.AddHours(offset);
			}else if (type == "d"){
				anotherDate = baseDate.AddDays(offset);
			}else if (type == "w"){
				anotherDate = baseDate.AddDays(7*offset);
			}else if (type == "m"){
				anotherDate = baseDate.AddMonths(offset);
			}
		
			if (anotherDate < baseDate){
				result.Start = anotherDate;
				result.Finish = baseDate;
			}
			else{
				result.Start = baseDate;
				result.Finish = anotherDate;
			}
		}


		private static void GetLastWork(DateTime baseDate, DateTime[] holidays, DateRange result){
//специальный диапазон, закрывающий условное рабочее "вчера и сегодня", при этом вчера переходит через выходные при необходимости
			//если указан массив выходных дней, то использует его для оценки выходных
			var daybefore = baseDate.Date.AddDays(-1);
			while (true){
				if (null != holidays && 0 != holidays.Length){
					if (Array.IndexOf(holidays, daybefore) == -1) break;
				}
				else{
					if (daybefore.DayOfWeek != DayOfWeek.Sunday && daybefore.DayOfWeek != DayOfWeek.Saturday) break;
				}
				daybefore = daybefore.Date.AddDays(-1);
			}
			result.Start = daybefore;
			result.Finish = baseDate.Date.AddDays(1).AddSeconds(-1);
		}

		private static void GetYesterDay(DateTime baseDate, DateRange result){
//вчера это с 00:00 до 23:59:59 вчерашнего дня
			result.Start = baseDate.Date.AddDays(-1);
			result.Finish = baseDate.Date.AddSeconds(-1);
		}

		private static void GetToDay(DateTime baseDate, DateRange result){
//сегодня это с 00:00 до 23:59:59 сегодняшнего дня, не обязательно по "сей момент" чтобы избегать расхождения а показаниях часов на хостах
			result.Start = baseDate.Date;
			result.Finish = baseDate.Date.AddDays(1).AddSeconds(-1);
		}
	}
}
