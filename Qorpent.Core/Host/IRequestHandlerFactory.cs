using System;
using System.Net;
using Qorpent.IO.Http;

namespace Qorpent.Host{

    public static class RequestHandlerFactoryExtensions
    {
        public static IHostServer Handle(this IHostServer server, string url, IRequestHandler handler)
        {
            server.Factory.Register(url,handler);
            return server;
        }
    }

    /// <summary>
	/// </summary>
	public interface IRequestHandlerFactory{
		/// <summary>
		///     Возвращает хэндлер, соответствующий запросу
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		IRequestHandler GetHandler(IHostServer server, WebContext context, string callbackEndPoint);

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		/// <param name="uri"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		IRequestHandler GetHandler(IHostServer server, Uri uri, string callbackEndPoint);

		/// <summary>
		///     Регистрирует хэндлер для определенного адреса
		/// </summary>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		void Register(string path, IRequestHandler handler);
	}
}