namespace Qorpent.Report {
	/// <summary>
	///		Интерфейс фабрики отчётов
	/// </summary>
	public interface IReportFactory {
		/// <summary>
		///		Текущая конфигурация фабрики
		/// </summary>
		ReportFactoryConfig Config { get; }
		/// <summary>
		///		Сборка отчёта в виде элемента отчёта
		/// </summary>
		/// <param name="query">Запрос на отчёт</param>
		/// <returns>Элемент отчёта</returns>
		IContentItem GetContentItem(ReportQuery query);
		/// <summary>
		///		Инициализация фабрики
		/// </summary>
		/// <param name="config">Конфигурация фабрики</param>
		void Initialize(ReportFactoryConfig config);
	}
}