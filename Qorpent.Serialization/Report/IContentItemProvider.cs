using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.Report {
	/// <summary>
	///		Провайдер элементов отчёта
	/// </summary>
	public interface IContentItemProvider {
		/// <summary>
		///		Получение единицы отчёта
		/// </summary>
		/// <returns>Единица отчёта</returns>
		IEnumerable<IContentItem> Get(IConfig query);
		/// <summary>
		///		Инициализация провайдера контента
		/// </summary>
		/// <param name="factory">Фабрика</param>
		void Initialize(IReportFactory factory);
		/// <summary>
		///		Определяет признак того, что провайдер может обработать запрос
		/// </summary>
		/// <param name="query">Исследуемый запрос</param>
		/// <returns>Признак того, что провайдер может обработать запрос</returns>
		bool IsSupport(IConfig query);
	}
}