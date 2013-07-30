namespace Qorpent.IO.Resources {
	/// <summary>
	/// Состояния  отклика
	/// </summary>
	public enum ResourceResponseState {
		/// <summary>
		/// Неопределенный
		/// </summary>
		Undefined = 1,
		/// <summary>
		/// В процессе работы
		/// </summary>
		Busy = 4,
		/// <summary>
		/// Завершен
		/// </summary>
		Finished = 8,
		/// <summary>
		/// Ошибка
		/// </summary>
		Error = 16,
	}
}