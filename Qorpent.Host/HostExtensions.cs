using System;
using System.IO;
using System.Net;
using System.Text;
using Qorpent.Host.Handlers;

namespace Qorpent.Host
{
	/// <summary>
	/// Расширения фабрики хэндлеров
	/// </summary>
	public static class HostExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		public static void Finish(this HttpListenerContext context, string content, string mimeType = "text/plain", int status = 200)
		{
			var response = context.Response;
			Finish(response, content, mimeType, status);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		public static void Finish(this HttpListenerResponse response, string content, string mimeType="text/plain", int status=200)
		{
			response.StatusCode = status;
			response.ContentType = mimeType;
			var buffer = Encoding.UTF8.GetBytes(content);
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.Close();
		}

		/// <summary>
		/// Регистрирует статический хэндлер со статусом
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public static IHostServer On(this IHostServer server, string path, string content, string mimeType = "text/plain", int status = 200)
		{
			server.Factory.On(path, content,mimeType,status);
			return server;
		}

		/// <summary>
		/// Регистрирует статический хэндлер со статусом
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory On(this IRequestHandlerFactory factory,string path, string content, string mimeType = "text/plain", int status = 200)
		{
			factory.Register(path, new StaticHandler(content, mimeType, status));
			return factory;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="usonHandler"></param>
		/// <returns></returns>
		public static IHostServer On(this IHostServer server, string path, Func<dynamic, object> usonHandler)
		{
			server.Factory.On(path, usonHandler);
			return server;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="usonHandler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory On(this IRequestHandlerFactory factory, string path, Func<dynamic, object> usonHandler)
		{
			factory.Register(path, new UsonHandler(usonHandler));
			return factory;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IHostServer OnResponse(this IHostServer server, string path, Action<HttpListenerResponse> handler)
		{
			server.Factory.OnResponse(path, handler);
			return server;
		}

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory OnResponse(this IRequestHandlerFactory factory, string path, Action<HttpListenerResponse> handler )
		{
			factory.Register(path, new DelegateHandler((s, c, e, cn) => handler(c.Response)));
			return factory;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IHostServer OnContext(this IHostServer server, string path, Action<HttpListenerContext> handler)
		{
			server.Factory.OnContext(path, handler);
			return server;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IRequestHandlerFactory OnContext(this IRequestHandlerFactory factory, string path, Action<HttpListenerContext> handler)
		{
			factory.Register(path, new DelegateHandler((s, c, e, cn) => handler(c)));
			return factory;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="stream"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		public static void Finish(this HttpListenerContext context, Stream stream, string mimeType="application/json",int status=200){
			Finish(context.Response,stream,mimeType,status);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="stream"></param>
		/// <param name="mimeType"></param>
		/// <param name="status"></param>
		public static void Finish(this HttpListenerResponse response, Stream stream, string mimeType,int status)
		{
			response.StatusCode = status;
			response.ContentType = mimeType;
			stream.CopyTo(response.OutputStream,2^10);
			response.OutputStream.Flush();
			response.Close();
		}
	}
}