using Qorpent.Applications;
using Qorpent.IoC;

namespace Qorpent.Host{
	/// <summary>
	///     Интерфейс хоста
	/// </summary>
	public interface IHostServer{
		/// <summary>
		///     Состояние сервера
		/// </summary>
		HostServerState State { get; }

        /// <summary>
        /// 
        /// </summary>
        IContainer Container { get; }

		/// <summary>
		///     Конфигурация
		/// </summary>
		HostConfig Config { get; set; }

		/// <summary>
		///     Контекст приложения
		/// </summary>
		IApplication Application { get; }

		/// <summary>
		///     Фабрика хэндлеров
		/// </summary>
		IRequestHandlerFactory Factory { get; }

		/// <summary>
		///     Служба доступа к статическим ресурсам
		/// </summary>
		IHostServerStaticResolver Static { get; }

		/// <summary>
		/// </summary>
		IHostAuthenticationProvider Auth { get; }

		/// <summary>
		/// </summary>
		IEncryptProvider Encryptor { get; }


		/// <summary>
		///     Запускает сервер
		/// </summary>
		void Start();

		/// <summary>
		///     Завершает работу веб-сервера
		/// </summary>
		void Stop();

	    /// <summary>
	    ///     Инициализирует сервер
	    /// </summary>
	    void Initialize();
	}
}