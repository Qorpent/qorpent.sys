using System;

namespace Qorpent.Wiki {
	/// <summary>
	/// Тип объекта Wiki
	/// </summary>
	[Flags]
	public enum WikiObjectType {
		/// <summary>
		/// Тип не указан
		/// </summary>
		None = 0,
		/// <summary>
		/// Страница
		/// </summary>
		Page = 1,
		/// <summary>
		/// Файл
		/// </summary>
		File = 2,
		/// <summary>
		/// Все типы
		/// </summary>
		All = Page | File,
		
	}
}