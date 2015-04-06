using System;

namespace Qorpent.Utils{
	/// <summary>
	/// Описатель диапазона дат
	/// </summary>
	public class DateRange{
		/// <summary>
		/// Диапазон по умолчанию
		/// </summary>
		public DateRange (DateTime basis = default (DateTime), string expr = ""){
			Base = basis;
			Start = basis;
			Finish = basis;
			Expression = expr;
		}
		
		/// <summary>
		/// Выражение, использованное для получения диапазона
		/// </summary>
		public string Expression;
		/// <summary>
		/// Базисное время
		/// </summary>
		public DateTime Base;
		/// <summary>
		/// Время начала рейнджа
		/// </summary>
		public DateTime Start;
		/// <summary>
		/// Время окончания рейнджа
		/// </summary>
		public DateTime Finish;
		/// <summary>
		/// 
		/// </summary>
		public TimeSpan StartOffset{
			get { return Start - Base; }
			set { Start = Base + value; }
			
		}
		/// <summary>
		/// 
		/// </summary>
		public TimeSpan FinishOffset{
			get { return Finish - Base; }
			set { Finish = Base + value; }
			
		}
		/// <summary>
		/// 
		/// </summary>
		public TimeSpan Duration{
			get { return Finish - Start; }
		}
		/// <summary>
		/// Общетекстовое представление
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return string.Format("{0} ; {1} ( {2} ; {3} )",
			                     Start.ToString("yyyy-MM-dd HH:mm:ss"),
			                     Finish.ToString("yyyy-MM-dd HH:mm:ss"),
			                     Base.ToString("yyyy-MM-dd HH:mm:ss"),
			                     Expression
				);
		}
	}
}