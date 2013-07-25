namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Классы символов Json
	/// </summary>
	public enum CType {
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
		/// Запятая
		/// </summary>
		Сom,
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
	}
}