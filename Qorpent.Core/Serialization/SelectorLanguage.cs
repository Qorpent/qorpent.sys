namespace Qorpent.Serialization {
	/// <summary>
	///     Варианты языков для запросов
	/// </summary>
	public enum SelectorLanguage {
		/// <summary>
		///     Автоматическое определение
		/// </summary>
		Auto,
		/// <summary>
		///     Нативный CSS
		/// </summary>
		Css,
		/// <summary>
		///     Нативный  XPath
		/// </summary>
		XPath,

		/// <summary>
		///     Регулярное выражение для поиска в теле элемента
		/// </summary>
		Regex,

		/// <summary>
		///     Пользовательский вариант селектора
		/// </summary>
		Custom,
	}
}