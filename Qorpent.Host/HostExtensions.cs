using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Qorpent.Host.Handlers;
using Qorpent.IO.Http;

namespace Qorpent.Host{
	/// <summary>
	///     Расширения фабрики хэндлеров
	/// </summary>
	public static class HostExtensions{
		

		/// <summary>
		///     Регистрирует статический хэндлер со статусом
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public static IHostServer On(this IHostServer server, string path, string content, string mimeType = "text/plain",
		                             int status = 200){
			server.Factory.On(path, content, mimeType, status);
			return server;
		}

		/// <summary>
		///     Регистрирует статический хэндлер со статусом
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory On(this IRequestHandlerFactory factory, string path, string content,
		                                        string mimeType = "text/plain", int status = 200){
			factory.Register(path, new StaticHandler(content, mimeType, status));
			return factory;
		}

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="usonHandler"></param>
		/// <returns></returns>
		public static IHostServer On(this IHostServer server, string path, Func<dynamic, object> usonHandler){
			server.Factory.On(path, usonHandler);
			return server;
		}

		/// <summary>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="usonHandler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory On(this IRequestHandlerFactory factory, string path,
		                                        Func<dynamic, object> usonHandler){
			factory.Register(path, new UsonHandler(usonHandler));
			return factory;
		}

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IHostServer OnResponse(this IHostServer server, string path, Action<IHttpResponseDescriptor> handler){
			server.Factory.OnResponse(path, handler);
			return server;
		}


		/// <summary>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory OnResponse(this IRequestHandlerFactory factory, string path,
		                                                Action<IHttpResponseDescriptor> handler){
			factory.Register(path, new DelegateHandler((s, c, e, cn) => handler(c.Response)));
			return factory;
		}

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IHostServer OnContext(this IHostServer server, string path, Action<WebContext> handler){
			server.Factory.OnContext(path, handler);
			return server;
		}

		/// <summary>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory OnContext(this IRequestHandlerFactory factory, string path,
		                                               Action<WebContext> handler){
			factory.Register(path, new DelegateHandler((s, c, e, cn) => handler(c)));
			return factory;
		}

		
	}
}