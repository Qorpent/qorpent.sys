using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Единица значения
	/// </summary>
	public class DataItem {
		/// <summary>
		/// Исходное значение
		/// </summary>
		public decimal Value { get; set; }
		
		/// <summary>
		/// Коллектор позитивных сумм при нормализации
		/// </summary>
		public decimal PosMax { get; set; }
		/// <summary>
		/// Коллектор негативных сумм при нормализации
		/// </summary>
		public decimal NegMin { get; set; }
		/// <summary>
		/// Коллектор позитивных сумм при нормализации
		/// </summary>
		public decimal PosMin { get; set; }
		/// <summary>
		/// Коллектор негативных сумм при нормализации
		/// </summary>
		public decimal NegMax { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal Max {
			get { return Math.Max(NegMax < 0 ? NegMax : decimal.MinValue, PosMax > 0 ?PosMax:decimal.MinValue); }
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal Min {
			get {
				var result =  Math.Min(NegMin < 0 ? NegMin :  decimal.MaxValue, PosMin>=0? PosMin :  decimal.MaxValue);
				if (result == decimal.MaxValue) {
					return PosMax;
				}
				return result;
			}
		}

		/// <summary>
		/// Нормализованное значение
		/// </summary>
		public decimal NormalizedValue { get; set; }
		/// <summary>
		/// Положение лычки
		/// </summary>
		public LabelPosition LabelPosition { get; set; }

	}
}