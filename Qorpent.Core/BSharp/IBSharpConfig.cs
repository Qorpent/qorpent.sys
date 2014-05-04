using System.Collections.Generic;
using Qorpent.Config;
using Qorpent.Log;

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

		/// <summary>
		/// Журнал проекта
		/// </summary>
		IUserLog Log { get; set; }

		/// <summary>
		/// Элементы корневого уровня для игнора
		/// </summary>
		string[] IgnoreElements { get; set; }

		/// <summary>
		///Глобальные константы
		/// </summary>
		IConfig Global { get; set; }
		/// <summary>
		/// Опция обработки конструкций Requires
		/// </summary>
		bool DoProcessRequires { get; set; }
		/// <summary>
		/// Признак сохранения исходно лексической информации
		/// </summary>
		bool KeepLexInfo { get; set; }
	}
}