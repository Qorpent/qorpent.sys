using System.Collections.Generic;
using Qorpent.Applications;

namespace Qorpent.Host
{
	/// <summary>
	/// Интерфейс хоста
	/// </summary>
	public interface IHostServer
	{
		/// <summary>
		/// Состояние сервера
		/// </summary>
		HostServerState State { get; }

		/// <summary>
		/// Конфигурация
		/// </summary>
		HostConfig Config { get; set; }

		/// <summary>
		/// Контекст приложения
		/// </summary>
		IApplication Application { get; }

		/// <summary>
		/// Фабрика хэндлеров
		/// </summary>
		IRequestHandlerFactory Factory { get; }
		/// <summary>
		/// Служба доступа к статическим ресурсам
		/// </summary>
		IHostServerStaticResolver Static { get; }

	    

	    /// <summary>
		/// Запускает сервер
		/// </summary>
		void Start();

		/// <summary>
		/// Завершает работу веб-сервера
		/// </summary>
		void Stop();

	}
}