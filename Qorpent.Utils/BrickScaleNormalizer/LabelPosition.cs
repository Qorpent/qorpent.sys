using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// тип шкалы значений
	/// </summary>
	[Flags]
	public enum LabelPosition {
		/// <summary>
		/// Автоматически
		/// </summary>
		Auto,
		/// <summary>
		///  Сверху
		/// </summary>
		Above,
		/// <summary>
		/// Снизу
		/// </summary>
		Below,
		/// <summary>
		/// Скрытый
		/// </summary>
		Hidden ,
	}
}