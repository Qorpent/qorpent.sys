using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Состояния запроса
	/// </summary>
	[Flags]
	public enum ResourceRequestState {
		/// <summary>
		/// Неопределенный
		/// </summary>
		Undefined=1,
		/// <summary>
		/// Создан
		/// </summary>
		Created =2,
		/// <summary>
		/// В процессе работы
		/// </summary>
		Busy =4,
		/// <summary>
		/// Завершен
		/// </summary>
		Finished = 8,
		/// <summary>
		/// Ошибка
		/// </summary>
		Error =16,
	}
}