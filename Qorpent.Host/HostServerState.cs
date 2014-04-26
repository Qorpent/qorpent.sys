namespace Qorpent.Host
{
	/// <summary>
	/// Состояния сервера
	/// </summary>
	public enum HostServerState
	{
		/// <summary>
		/// Начальное состояние
		/// </summary>
		Initial,
		/// <summary>
		/// Инициализированное состояние
		/// </summary>
		Initalized,
		/// <summary>
		/// В процессе работы
		/// </summary>
		Run,
		/// <summary>
		/// Останов
		/// </summary>
		Stopped,
		/// <summary>
		/// Необработанное исключение
		/// </summary>
		Fail
	}
}