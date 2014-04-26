using System;
using System.Net;

namespace Qorpent.Host
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRequestHandlerFactory 
	{
		/// <summary>
		/// Возвращает хэндлер, соответствующий запросу
		/// </summary>
		/// <param name="server"></param>
		/// <param name="context"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		IRequestHandler GetHandler(IHostServer server, HttpListenerContext context, string callbackEndPoint);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="uri"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		IRequestHandler GetHandler(IHostServer server, Uri uri, string callbackEndPoint);
		/// <summary>
		/// Регистрирует хэндлер для определенного адреса
		/// </summary>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		void Register(string path, IRequestHandler handler);
	}

	
}