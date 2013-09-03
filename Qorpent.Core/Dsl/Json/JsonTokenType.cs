using System;

namespace Qorpent.Json {
	/// <summary>
	/// Типы токенов
	/// </summary>
	[Flags]
	public enum JsonTokenType {

		/// <summary>
		/// Неопределенный
		/// </summary>
		None = 0,
		/// <summary>
		/// Открытие блока
		/// </summary>
		BeginObject = 1,
		/// <summary>
		/// Закрытие блока
		/// </summary>
		CloseObject = 2,
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
		Literal =16,
		/// <summary>
		/// Строка
		/// </summary>
		String =32,
		/// <summary>
		/// Число
		/// </summary>
		Number = 64,
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