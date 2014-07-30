using System;

namespace Qorpent.Model {
	/// <summary>
	///		Способ суммирования
	/// </summary>
	[Flags]
	public enum SummaType {
		/// <summary>
		///		Неопределено
		/// </summary>
		Undefined = 0,
		/// <summary>
		///		Без суммирования
		/// </summary>
		None = 1,
		/// <summary>
		///		Суммирование
		/// </summary>
		Summa = 2,
		/// <summary>
		///		Маркер групповых сумм
		/// </summary>
		Group = 4,
		/// <summary>
		///		Маркер объекта, не подлежащего суммированию в других суммах
		/// </summary>
		NoSum = 8,
		/// <summary>
		///		Значение по умолчанию
		/// </summary>
		Default = None
	}
}
