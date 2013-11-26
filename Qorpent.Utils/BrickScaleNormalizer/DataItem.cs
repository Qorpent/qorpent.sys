using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

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
				if (null != Second)
				{
					if (Second.LabelPosition == LabelPosition.Auto) return false;
				}
				return true;
			}
		}
		
	}
	/// <summary>
	/// Колонка значений графика для расчета их позиций
	/// </summary>
	public class DataItemColon {
		/// <summary>
		/// 
		/// </summary>
		public decimal ScaleMax { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DataItemLabelCollision> FindCollisions() {
			DataItem previous = null;
			DataItem current = null;
			for (var i = 0; i < Items.Length; i++) {
				if(Items[i].LabelPosition==LabelPosition.Hidden)continue;
				previous = current;
				current = Items[i];
				if (0 == i && current.NormalizedLabelMin < 0) {
					yield return new DataItemLabelCollision{ScaleMax = this.ScaleMax,First = current,Second = null};
				}
				if (null != previous && (previous.NormalizedLabelMax > current.NormalizedLabelMin) &&
				    (current.NormalizedValue > previous.NormalizedLabelMax)) {
						yield return new DataItemLabelCollision { ScaleMax = this.ScaleMax, First = previous, Second = current };
				}
				if (null != previous && (current.NormalizedLabelMax > previous.NormalizedLabelMin) &&
					(previous.NormalizedValue >  current.NormalizedLabelMax))
				{
					yield return new DataItemLabelCollision { ScaleMax = this.ScaleMax, First = current, Second = previous };
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public DataItem[] Items { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public decimal GetTemperature() {
			return FindCollisions().Select(_ => _.Overlap).Sum();
		}

		/// <summary>
		/// 
		/// </summary>
		public void TryMinimizeTemperature() {
			if (GetTemperature() == 0) return;
			ResetTemperature();
			int variant = 0;
			decimal currentMin = GetTemperature();
			var keepTopAndBottom = KeepTopAndBottom();
			if (keepTopAndBottom < currentMin)
			{
				variant = 1;
				currentMin = keepTopAndBottom;
			}
			ResetTemperature();
			var fromdownVariant = ColdFromDown();
			if (fromdownVariant < currentMin) {
				variant = 2;
				currentMin = fromdownVariant;
			}
			ResetTemperature();
			var fromUpVariant = ColdFromUp();
			if (fromUpVariant < currentMin)
			{
				variant = 3;
				currentMin = fromUpVariant;
			}
			ResetTemperature();
			var threeWorst = ColdFromThreeWorst();
			if (threeWorst < currentMin)
			{
				variant = 4;
			}
			if (0 == variant) {
				foreach (var dataItem in Items) {
					dataItem.LabelPosition = LabelPosition.Auto;
				}
			}
			else if (1 == variant)
			{
				KeepTopAndBottom();
			}
			else if(2==variant) {
				ColdFromDown();
			}else if (3 == variant) {
				ColdFromUp();
			}
			else {
				ColdFromThreeWorst();
			}
		}

		private void ResetTemperature() {
			foreach (var dataItem in Items) {
				dataItem.LabelPosition = LabelPosition.Auto;
			}
		}

		private decimal KeepTopAndBottom() {
			var collisions = FindCollisions().ToArray();
			var fst = collisions.FirstOrDefault(_ => _.Second == null);
			var sec = collisions.FirstOrDefault(_ => _.First == null);
			if (null != fst) {
				fst.First.LabelPosition = LabelPosition.Above;
			}
			if (null != sec) {
				sec.Second.LabelPosition = LabelPosition.Below;
			}
			collisions = FindCollisions().Where(_ => null != _.First && null != _.Second ).ToArray();
			foreach (var collision in collisions) {
				if (collision.First == fst.First && collision.Second == sec.Second) {
					continue;
				}
				if (collision.First == fst.First) {
					collision.Second.LabelPosition = LabelPosition.Above;
				}
				else if (collision.Second == sec.Second) {
					collision.First.LabelPosition = LabelPosition.Below;
				}
				else {
					collision.Second.LabelPosition= LabelPosition.Above;
				}
			}

			return GetTemperature();
		}

		private decimal ColdFromThreeWorst() {
			for (var i = 0; i < 3; i++) {
				var collision = FindCollisions().OrderByDescending(_ => _.Overlap).FirstOrDefault();
				if (null == collision) break;
				if (collision.Second == null) {
					collision.First.LabelPosition = LabelPosition.Above;
				}else if (collision.First == null) {
					collision.Second.LabelPosition = LabelPosition.Below;
				}
				else {
					collision.Second.LabelPosition = LabelPosition.Above;
					collision.First.LabelPosition = LabelPosition.Below;
				}
			}
			return GetTemperature();
		}

		private decimal ColdFromUp() {
			throw new NotImplementedException();
		}

		private decimal ColdFromDown() {
			throw new NotImplementedException();
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
		/// 
		/// </summary>
		public decimal NormalizedLabelMin { 
			get {
				if (LabelPosition != LabelPosition.Above) return NormalizedValue - DEFAULT_LABEL_HEIGHT;
				return NormalizedValue;
			} 
		}
		/// <summary>
		/// Максимальный размер с учетом "лычек"
		/// </summary>
		public decimal NormalizedLabelMax {
			get {
				if (LabelPosition != LabelPosition.Below) return NormalizedValue + DEFAULT_LABEL_HEIGHT;
				return NormalizedValue;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected const int DEFAULT_LABEL_HEIGHT = 20;

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