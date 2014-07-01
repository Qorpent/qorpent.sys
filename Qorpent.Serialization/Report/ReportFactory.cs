using System.Linq;
using Qorpent.Applications;

namespace Qorpent.Report {
	/// <summary>
	///		Общая фабрика отчётной системы
	/// </summary>
	public class ReportFactory : IReportFactory {
		/// <summary>
		///		Список провайдеров
		/// </summary>
		private IContentItemProvider[] _providers;
		/// <summary>
		///		Текущий конфиг фабрики
		/// </summary>
		public ReportFactoryConfig Config { get; private set; }
		/// <summary>
		///		Сборка отчёта в виде элемента отчёта
		/// </summary>
		/// <param name="query">Запрос на отчёт</param>
		/// <returns>Элемент отчёта</returns>
		public IContentItem GetContentItem(ReportQuery query) {
			IContentItemProvider provider = null;
			
			foreach (var contentProvider in _providers) {
				if (contentProvider.IsSupport(query)) {
					provider = contentProvider;
					break;
				}
			}
			
			if (provider != null) {
				return provider.Get(query).First();
			}
			return null;
		}
		/// <summary>
		///		Инициализация фабрики
		/// </summary>
		/// <param name="config">Конфигурация фабрики</param>
		public void Initialize(ReportFactoryConfig config) {
			Config = config;
			_providers = Application.Current.Container.All<IContentItemProvider>().ToArray();

			foreach (var provider in _providers) {
				provider.Initialize(this);
			}
		}
	}
}
