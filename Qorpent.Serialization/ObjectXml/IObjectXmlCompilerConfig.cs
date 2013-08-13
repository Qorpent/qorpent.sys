using Qorpent.Config;

namespace Qorpent.ObjectXml {
	/// <summary>
	///     Интерфейс конфигурации компилятора
	/// </summary>
	public interface IObjectXmlCompilerConfig : IConfig {
		/// <summary>
		///     Флаг использования интерполяции
		/// </summary>
		bool UseInterpolation { get; set; }

		/// <summary>
		///     Если включено все исходники рассматриваются как один большой источник
		///     с общим индексом
		/// </summary>
		bool SingleSource { get; set; }
	}
}