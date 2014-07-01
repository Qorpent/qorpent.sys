using Qorpent.Config;

namespace Qorpent.Report{
	/// <summary>
	///		Конфиг фабрики отчётов
	/// </summary>
	public class ReportFactoryConfig : ConfigBase{
		/// <summary>
		///		Источник конфигурации
		/// </summary>
		public IXmlConfigSource Source {
			get { return Get<IXmlConfigSource>("sources"); }
			set { Set("sources", value); }
		}
	}
}