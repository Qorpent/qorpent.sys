using System.Net;
using Qorpent.IO.Http;

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
        void Authenticate(HttpRequestDescriptor request, HttpResponseDescriptor response);

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
        void Authenticate(HttpRequestDescriptor request, HttpResponseDescriptor response, string username, string password);

		/// <summary>
		///     Выполняет вход с разбором переданных параметров
		/// </summary>
		/// <param name="context"></param>
		void Logon(HttpRequestDescriptor request,HttpResponseDescriptor response);

		/// <summary>
		///     Выполняет выход из контекста
		/// </summary>
		/// <param name="context"></param>
        void Logout(HttpRequestDescriptor request, HttpResponseDescriptor response);

		/// <summary>
		/// Проверка аутентифицированного контекста
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
        bool IsAuth(HttpRequestDescriptor request, HttpResponseDescriptor response);
	}
}