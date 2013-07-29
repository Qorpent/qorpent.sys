using System;

namespace Qorpent.Json {
	/// <summary>
	/// Классы символов Json
	/// </summary>
	[Flags]
	public enum CType {
		/// <summary>
		/// Zero type
		/// </summary>
		None,
		/// <summary>
		/// Апостроф
		/// </summary>
		Apos,
		/// <summary>
		/// Кавычки
		/// </summary>
		Quot,
		/// <summary>
		/// Escape
		/// </summary>
		Esc,
		
		/// <summary>
		/// Двоеточие
		/// </summary>
		Col,
		/// <summary>
		/// Точка
		/// </summary>
		Dot,
		/// <summary>
		/// Число
		/// </summary>
		Dig,
		/// <summary>
		/// Букво-цифра
		/// </summary>
		Lit,
		/// <summary>
		/// Спецсимвол
		/// </summary>
		NLit,
		/// <summary>
		/// Открытие блока
		/// </summary>
		Op,
		/// <summary>
		/// Закрытие блока
		/// </summary>
		Cl,
		/// <summary>
		/// Открытие массива
		/// </summary>
		OpA,
		/// <summary>
		/// Закрытие массива
		/// </summary>
		ClA,
		/// <summary>
		/// Новая строка
		/// </summary>
		NL,
		/// <summary>
		/// Пробельные символы
		/// </summary>
		WS,
		/// <summary>
		/// Знак минуса
		/// </summary>
		Min,
		/// <summary>
		/// Запятая
		/// </summary>
		Com,
	}
}