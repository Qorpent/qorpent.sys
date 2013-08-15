using System;

namespace Qorpent.Config {
	/// <summary>
	/// Тип генерации конфига в строку
	/// </summary>
	[Flags]
	public enum ConfigRenderType {
		/// <summary>
		/// Плоский BXL-совместимый элемент с атрибутами
		/// </summary>
		SimpleBxl = 1,
		/// <summary>
		/// По умолчанию- плоский BXL
		/// </summary>
		Default =SimpleBxl
	}
}