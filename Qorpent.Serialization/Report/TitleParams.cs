﻿using System;

namespace Qorpent.Report {
	/// <summary>
	///		Параметры заголовка
	/// </summary>
	public class TitleParams {
		/// <summary>
		///		Имя периода
		/// </summary>
		public string PeriodName { get; set; }
		/// <summary>
		///		Дата старта
		/// </summary>
		public DateTime StartDate { get; set; }
		/// <summary>
		///		Дата финиша
		/// </summary>
		public DateTime FinishDate { get; set; }
		/// <summary>
		///		Год
		/// </summary>
		public int Year { get; set; }
		/// <summary>
		///		Период
		/// </summary>
		public int Period { get; set; }
		/// <summary>
		///		Предыдущий год
		/// </summary>
		public int PrevYear {
			get { return Year - 1; }
		}
		/// <summary>
		///		Форматированная дата старта
		/// </summary>
		public string Start {
			get { return StartDate.ToString(QorpentConst.Date.DefaultFormat); }
		}
		/// <summary>
		/// 
		/// </summary>
		public int y {
			get { return Year; }
		}
		/// <summary>
		/// </summary>
		public string p {
			get { return PeriodName; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string sld {
			get { return StartPrevMonthLastDay; }
		}
		/// <summary>
		///		Форматированная дата финиша
		/// </summary>
		public string Finish {
			get { return FinishDate.ToString(QorpentConst.Date.DefaultFormat); }
		}
		/// <summary>
		///		
		/// </summary>
		public string StartPrevMonthLastDay {
			get { return new DateTime(StartDate.Year, StartDate.Month, 1).AddDays(-1).ToString(QorpentConst.Date.DefaultFormat); }
		}
		/// <summary>
		/// 
		/// </summary>
		public string FinishNextMonthFirstDay {
			get { return new DateTime(FinishDate.Year, FinishDate.Month + 1, 1).ToString(QorpentConst.Date.DefaultFormat); }
		}
		/// <summary>
		/// 
		/// </summary>
		public TitleParams() {
			StartDate = QorpentConst.Date.Begin;
			FinishDate = QorpentConst.Date.End;
		}
	}
}