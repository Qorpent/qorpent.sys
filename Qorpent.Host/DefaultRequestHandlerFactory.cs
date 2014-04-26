using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Qorpent.Host.Handlers;
using Qorpent.Host.Qweb;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultRequestHandlerFactory:ServiceBase,IRequestHandlerFactory
	{

		IDictionary<string,IRequestHandler> _cache = new Dictionary<string, IRequestHandler>();

		/// <summary>
		/// Возвращает хэндлер, соответствующий запросу
		/// </summary>
		/// <param name="server"></param>
		/// <param name="context"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		public IRequestHandler GetHandler(IHostServer server, HttpListenerContext context, string callbackEndPoint)
		{
			
			lock (this){
				var uri = context.Request.Url;
				return GetHandler(server, uri, callbackEndPoint);
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="uri"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		public IRequestHandler GetHandler(IHostServer server, Uri uri, string callbackEndPoint){
			var path = uri.AbsolutePath;
			if (path.IsEmpty() || path.Equals("/"))
			{
				return new StaticFileHandler();
			}
			if (path.Split('/').Last().Contains(".") && !path.EndsWith(".qweb"))
			{
				return new StaticFileHandler();
			}

			if (path.EndsWith("/xml"))
			{
				path = path.Substring(0, path.Length - 4);
			}
			if (path.StartsWith("/!") || path.StartsWith("/~") || path.StartsWith("/-"))
			{
				path = "/" + path.Substring(2);
			}
			if (_cache.ContainsKey(path))
			{
				return _cache[path];
			}
			if (path == "/wiki")
			{
				return new WikiHandler();
			}
			if (path.SmartSplit().Count >= 2 || path.EndsWith(".qweb"))
			{
				return new HostQwebHandler();
			}
			return new NotFoundHandler();
		}

		/// <summary>
		/// Регистрирует хэндлер для определенного адреса
		/// </summary>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		public void Register(string path, IRequestHandler handler)
		{
			lock (this)
			{
				_cache[path] = handler;
			}
		}
	}
}