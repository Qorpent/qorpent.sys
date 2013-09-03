using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Тип использования прокси
	/// </summary>
	[Flags]
	public enum ProxyUsage {
		/// <summary>
		/// Неопределенный тип
		/// </summary>
		Undefined = 1,
		/// <summary>
		/// Прокси выключен
		/// </summary>
		NoProxy = 2,
		/// <summary>
		/// Используется системный прокси
		/// </summary>
		System = 4,
		/// <summary>
		/// Используется особый прокси
		/// </summary>
		Custom = 8,
		/// <summary>
		/// По умолчанию - автоопределяемый системный прокси
		/// </summary>
		Default = System
	}
}