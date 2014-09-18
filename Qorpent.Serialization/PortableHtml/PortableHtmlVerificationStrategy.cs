namespace Qorpent.PortableHtml{
	/// <summary>
	///     Стратегия разбора схемы
	/// </summary>
	public enum PortableHtmlVerificationStrategy{
		/// <summary>
		///     Любой не None результат завершает проверку
		/// </summary>
		ForcedResult = 1,

		/// <summary>
		///     Первая ошибка на конкретном элементе прекращает его работу
		/// </summary>
		ForcedElementResult = 2,

		/// <summary>
		///     Обрабатываются и записываются все ошибки
		/// </summary>
		Full = 4,
	}
}