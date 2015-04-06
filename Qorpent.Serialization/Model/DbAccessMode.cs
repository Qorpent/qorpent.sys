using System;

namespace Qorpent.Scaffolding.Model {
	/// <summary>
	///		Режим доступа до сущности таблицы
	/// </summary>
	[Flags]
	public enum DbAccessMode {
		/// <summary>
		///		Только чтение
		/// </summary>
		Read = 1,
		/// <summary>
		///		Запись
		/// </summary>
		Write = 2,
		/// <summary>
		///		Чтение-запись
		/// </summary>
		ReadWrite = Read | Write
	}
}
