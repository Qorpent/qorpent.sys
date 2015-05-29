using System.Net;
using Qorpent.IO.Http;

namespace Qorpent.Host{
    /// <summary>
	/// </summary>
	public interface IHostAuthenticationProvider{
		/// <summary>
		///     Инициализирует провайдер аутентификации
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);
		/// <summary>
		/// </summary>
		/// <param name="context"></param>
        void Authenticate(WebContext context);
		/// <summary>
		///     Выполняет вход с разбором переданных параметров
		/// </summary>
		/// <param name="context"></param>
		void Logon(WebContext context);

		/// <summary>
		///     Выполняет выход из контекста
		/// </summary>
		/// <param name="context"></param>
        void Logout(WebContext context);
		/// <summary>
		/// Проверка аутентифицированного контекста
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
        void IsAuth(WebContext context);
	}
}