using System;

namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum DbGenerationMode{
		/// <summary>
		/// Неопределенный режим
		/// </summary>
		None =0,
		/// <summary>
		/// Признак генерации в режиме скрипта
		/// </summary>
		Script=1,
		/// <summary>
		/// Признак генерации в рамках процедуры
		/// </summary>
		Procedure=2,
		/// <summary>
		/// Признак генерации в защищенном режиме
		/// </summary>
		Safe=4,
		/// <summary>
		/// Особый режим с постепенным накатом части обновлений
		/// </summary>
		Patch=8,
		/// <summary>
		/// Скрипт удаления объектов
		/// </summary>
		Drop =16,
	}
}