using System;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Типы токенов
	/// </summary>
	[Flags]
	public enum TType {
		/// <summary>
		/// Открытие блока
		/// </summary>
		Open,
		/// <summary>
		/// Закрытие блока
		/// </summary>
		Close,
		/// <summary>
		/// Открытие массива
		/// </summary>
		OpenArray,
		/// <summary>
		/// Закрытие массива
		/// </summary>
		CloseArray,
		/// <summary>
		/// Литерал
		/// </summary>
		Lit,
		/// <summary>
		/// Строка
		/// </summary>
		Str,
		/// <summary>
		/// Число
		/// </summary>
		Num,
		/// <summary>
		/// Запятая
		/// </summary>
		Comma,
		/// <summary>
		/// Двоеточие
		/// </summary>
		Colon,
		/// <summary>
		/// Булевское значение
		/// </summary>
		Bool,

		/// <summary>
		/// Null
		/// </summary>
		Null,
	}
}