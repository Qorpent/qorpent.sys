using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	///		Элемент данных
	/// </summary>
	public class NumericDataItem : Tagged {
		/// <summary>
		///		Признак того, что занчение было проинициализировано
		/// </summary>
		public bool IsValueInitialized { get; private set; }
		/// <summary>
		///		Значение
		/// </summary>
		private decimal _value;
		/// <summary>
		///		Абсолютное значение
		/// </summary>
		public decimal AbsValue {
			get { return Math.Abs(Value); }
		}
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
		///		Элемент данных
		/// </summary>
		public NumericDataItem() {
			IsValueInitialized = false;
		}
	}
}