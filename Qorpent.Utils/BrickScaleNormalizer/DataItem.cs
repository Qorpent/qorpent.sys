using System;
using System.Globalization;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
	/// Единица значения
	/// </summary>
	public class DataItem {
		/// <summary>
		///		Значение
		/// </summary>
	    private decimal _value;
		/// <summary>
		///		Признак того, что занчение было проинициализировано
		/// </summary>
		public bool IsValueInitialized { get; set; }
	    /// <summary>
	    /// Исходное значение
	    /// </summary>
	    public decimal Value {
		    get { return _value; }
			set {
				_value = value;
				IsValueInitialized = true;
			}
	    }
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
        ///     Индекс внутри датасета (порядковый номер вставки)
        /// </summary>
        public int DatasetIndex { get; set; }
        /// <summary>
        ///     Нужно ли прятать данное значение
        /// </summary>
        public bool Hide { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal NormalizedLabelMin {
            get { return MatchNormalizedLabelMin(LabelPosition); } 
		}
		/// <summary>
		/// Максимальный размер с учетом "лычек"
		/// </summary>
		public decimal NormalizedLabelMax {
            get { return MatchNormalizedLabelMax(LabelPosition); }
		}
		/// <summary>
		///     Размер «лычки» в пикселях
		/// </summary>
		public int LabelHeight = 10;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelPosition"></param>
        /// <returns></returns>
        public decimal MatchNormalizedLabelMin(LabelPosition labelPosition) {
            if (labelPosition != LabelPosition.Above) return NormalizedValue - LabelHeight;
            return NormalizedValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelPosition"></param>
        /// <returns></returns>
        public decimal MatchNormalizedLabelMax(LabelPosition labelPosition) {
            if (labelPosition != LabelPosition.Below) return NormalizedValue + LabelHeight;
            return NormalizedValue;
        }
        /// <summary>
        ///     Приведение <see cref="DataItem"/> к строке
        /// </summary>
        /// <returns>Строковое представление <see cref="DataItem"/></returns>
        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
	}
}