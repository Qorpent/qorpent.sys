using System;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Типы токенов
	/// </summary>
	[Flags]
	public enum TType {

		/// <summary>
		/// Неопределенный
		/// </summary>
		None = 0,
		/// <summary>
		/// Открытие блока
		/// </summary>
		Open = 1,
		/// <summary>
		/// Закрытие блока
		/// </summary>
		Close = 2,
		/// <summary>
		/// Открытие массива
		/// </summary>
		OpenArray = 4,
		/// <summary>
		/// Закрытие массива
		/// </summary>
		CloseArray = 8,
		/// <summary>
		/// Литерал
		/// </summary>
		Lit =16,
		/// <summary>
		/// Строка
		/// </summary>
		Str =32,
		/// <summary>
		/// Число
		/// </summary>
		Num = 64,
		/// <summary>
		/// Запятая
		/// </summary>
		Comma = 128,
		/// <summary>
		/// Двоеточие
		/// </summary>
		Colon = 256,
		/// <summary>
		/// Булевское значение
		/// </summary>
		Bool =512,

		/// <summary>
		/// Null
		/// </summary>
		Null = 1024,
	}
}