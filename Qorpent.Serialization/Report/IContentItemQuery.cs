namespace Qorpent.Report {
	/// <summary>
	///		Запрос на элемент отчёта
	/// </summary>
	public interface IContentItemQuery {
		/// <summary>
		///		Код элемента контента
		/// </summary>
		string Code { get; set; }
		/// <summary>
		///		Набор условий для выбора колонок
		/// </summary>
		string Conditions { get; set; }
		/// <summary>
		///     Год
		/// </summary>
		int Year { get; set; }
		/// <summary>
		///     Период
		/// </summary>
		int Period { get; set; }
		/// <summary>
		///     Объект
		/// </summary>
		int Object { get; set; }
		/// <summary>
		///		высота
		/// </summary>
		int Height { get; set; }
	}
}