﻿using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс конфигурации BSharp
	/// </summary>
	public interface IBSharpConfig:IScope {
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
		IScope Global { get; set; }
		/// <summary>
		/// Опция обработки конструкций Requires
		/// </summary>
		bool DoProcessRequires { get; set; }
		/// <summary>
		/// Признак сохранения исходно лексической информации
		/// </summary>
		bool KeepLexInfo { get; set; }
        /// <summary>
        /// Базовое пространство имен
        /// </summary>
	    string DefaultNamespace { get; set; }

	    IDictionary<string, object> RequireMap { get; set; } 
	}
}