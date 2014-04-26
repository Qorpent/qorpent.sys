namespace Qorpent.Host{
	/// <summary>
	/// Интерфейс инициализации хоста
	/// </summary>
	public interface IHostServerInitializer{
		/// <summary>
		/// Инициализирует сервер
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);
	}
}