using System;

namespace Qorpent.Utils.BrickScaleNormalizer {

	/// <summary>
	/// Обхект пересечения "лычек"
	/// </summary>
	public class DataItemLabelCollision {
		/// <summary>
		/// 
		/// </summary>
		public decimal ScaleMax { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DataItem First { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DataItem Second { get; set; }
		/// <summary>
		/// Степень наложения
		/// </summary>
		public decimal Overlap {
			get {
				if (First != null && Second == null) {
					return 0 - First.NormalizedLabelMin;
				}
				if (Second != null && First == null) {
					return Second.NormalizedLabelMax - ScaleMax;
				}
				return First.NormalizedLabelMax - Second.NormalizedLabelMin;
			}
		}
		/// <summary>
		/// Признак фиксации
		/// </summary>
		public bool IsFixed {
			get {
				if (null != First) {
					if (First.LabelPosition == LabelPosition.Auto) return false;
				}

				if (null != Second) {
					if (Second.LabelPosition == LabelPosition.Auto) return false;
				}

				return true;
			}
		}
		
	}

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
        ///     Порядковый номер внутри ряда
        /// </summary>
        public int Index { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal NormalizedLabelMin { 
			get {
				if (LabelPosition != LabelPosition.Above) return NormalizedValue - DefaultLabelHeight;
				return NormalizedValue;
			} 
		}
		/// <summary>
		/// Максимальный размер с учетом "лычек"
		/// </summary>
		public decimal NormalizedLabelMax {
			get {
				if (LabelPosition != LabelPosition.Below) return NormalizedValue + DefaultLabelHeight;
				return NormalizedValue;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected const int DefaultLabelHeight = 20;

		/// <summary>
		/// 
		/// </summary>
		public decimal Max {
			get {
				var result = Math.Max(NegMax < 0 ? NegMax : decimal.MinValue, PosMax > 0 ?PosMax:decimal.MinValue);
				if (result == decimal.MinValue) {
					result = 0;
				}
				return result;
			}
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