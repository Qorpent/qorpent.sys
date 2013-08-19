using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс конфигурации BSharp
	/// </summary>
	public interface IBSharpConfig:IConfig {
		/// <summary>
		///     Признак использования интерполяции при компиляции
		/// </summary>
		bool UseInterpolation { get; set; }

		/// <summary>
		///     Если включено все исходники рассматриваются как один большой источник
		///     с общим индексом
		/// </summary>
		bool SingleSource { get; set; }
		/// <summary>
		/// Условия компиляции 
		/// </summary>
		IDictionary<string, string> Conditions { get; set; }
	}
}