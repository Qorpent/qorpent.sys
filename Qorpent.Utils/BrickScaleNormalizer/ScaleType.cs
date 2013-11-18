using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// тип шкалы значений
	/// </summary>
	[Flags]
	public enum ScaleType {
		/// <summary>
		/// Первая, основная
		/// </summary>
		First,
		/// <summary>
		/// Вторая, дополнительная
		/// </summary>
		Second,
	}
}