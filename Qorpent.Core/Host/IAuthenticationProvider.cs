using System.Net;

namespace Qorpent.Host{
	/// <summary>
	/// </summary>
	public interface IAuthenticationProvider{
		/// <summary>
		///     Инициализирует провайдер аутентификации
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		void Authenticate(HttpListenerContext context);

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		void Authenticate(HttpListenerContext context, string username, string password);

		/// <summary>
		///     Выполняет вход с разбором переданных параметров
		/// </summary>
		/// <param name="context"></param>
		void Logon(HttpListenerContext context);

		/// <summary>
		///     Выполняет выход из контекста
		/// </summary>
		/// <param name="context"></param>
		void Logout(HttpListenerContext context);

		/// <summary>
		/// Проверка аутентифицированного контекста
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool IsAuth(HttpListenerContext context);
	}
}