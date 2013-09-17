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
		None = 1,
		/// <summary>
		/// Апостроф
		/// </summary>
		Apos =1<<1,
		/// <summary>
		/// Кавычки
		/// </summary>
        Quot = 1 << 2,
		/// <summary>
		/// EscapeXmlName
		/// </summary>
        Esc = 1 << 3,
		
		/// <summary>
		/// Двоеточие
		/// </summary>
        Col = 1 << 4,
		/// <summary>
		/// Точка
		/// </summary>
        Dot = 1 << 5,
		/// <summary>
		/// Число
		/// </summary>
        Dig = 1 << 6,
		/// <summary>
		/// Букво-цифра
		/// </summary>
        Lit = 1 << 7,
		/// <summary>
		/// Спецсимвол
		/// </summary>
        NLit = 1 << 8,
		/// <summary>
		/// Открытие блока
		/// </summary>
        Op = 1 << 9,
		/// <summary>
		/// Закрытие блока
		/// </summary>
        Cl = 1 << 10,
		/// <summary>
		/// Открытие массива
		/// </summary>
        OpA = 1 << 11,
		/// <summary>
		/// Закрытие массива
		/// </summary>
        ClA = 1 << 12,
		/// <summary>
		/// Новая строка
		/// </summary>
        NL = 1 << 13,
		/// <summary>
		/// Пробельные символы
		/// </summary>
        WS = 1 << 14,
		/// <summary>
		/// Знак минуса
		/// </summary>
        Min = 1 << 16,
		/// <summary>
		/// Запятая
		/// </summary>
        Com = 1 << 17,
	}
}