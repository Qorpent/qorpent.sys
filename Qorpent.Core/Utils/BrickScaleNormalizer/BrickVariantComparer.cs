using System.Collections.Generic;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Класс сравнения вариантов
	/// </summary>
	public class BrickVariantComparer:IComparer<BrickVariant> {
		int IComparer<BrickVariant>.Compare(BrickVariant x, BrickVariant y) {
			return x.CompareTo(y);
		}
	}
}