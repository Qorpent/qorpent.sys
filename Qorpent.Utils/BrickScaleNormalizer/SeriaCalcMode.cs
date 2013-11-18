using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Режим вычисления
	/// </summary>
	[Flags]
	public enum SeriaCalcMode {
		/// <summary>
		/// Суммировать внутри серии
		/// </summary>
		SeriaSum =1 ,
		/// <summary>
		/// Линейно рассматривать серию
		/// </summary>
		SeriaLinear=2,
		/// <summary>
		/// Суммировать между сериями
		/// </summary>
		CrossSeriaSum=4,
		/// <summary>
		/// Линейно рассматривать серии
		/// </summary>
		CrossSeriaLinear=8,
		/// <summary>
		/// Однорядный график
		/// </summary>
		Linear =  SeriaLinear | CrossSeriaLinear,
		/// <summary>
		/// Обычный полный стековый
		/// </summary>
		Stacked =  SeriaSum | CrossSeriaSum,
		/// <summary>
		/// Стековый мультисерийный
		/// </summary>
		MultiSeriaStacked = SeriaSum | CrossSeriaLinear,
	}
}