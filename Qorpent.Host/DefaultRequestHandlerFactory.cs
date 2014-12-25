using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using Qorpent.Host.Handlers;
using Qorpent.Host.Qweb;
using Qorpent.IO.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
	/// <summary>
	/// </summary>
	public class DefaultRequestHandlerFactory : ServiceBase, IRequestHandlerFactory{
		private readonly IDictionary<string, IRequestHandler> _cache = new Dictionary<string, IRequestHandler>();

		/// <summary>
		///     Возвращает хэндлер, соответствующий запросу
		/// </summary>
		/// <param name="server"></param>
		/// <param name="context"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		public IRequestHandler GetHandler(IHostServer server, HttpListenerContext context, string callbackEndPoint){
			lock (this){
				Uri uri = context.Request.Url;
				return GetHandler(server, uri, callbackEndPoint);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		/// <param name="uri"></param>
		/// <param name="callbackEndPoint"></param>
		/// <returns></returns>
		public IRequestHandler GetHandler(IHostServer server, Uri uri, string callbackEndPoint){
			string path = uri.AbsolutePath;

		    if (server.Config.Proxize.Keys.Any(_ => path.StartsWith(_))) {
		        return new ProxyHandler();
		    }

			if (path.IsEmpty() || path.Equals("/")){
				return new StaticFileHandler();
			}
			
			if (path.EndsWith("/xml")){
				path = path.Substring(0, path.Length - 4);
			}
			if (_cache.ContainsKey(path))
			{
				return _cache[path];
			}
			if (path.Split('/').Last().Contains(".") && !path.EndsWith(".qweb"))
			{
				return new StaticFileHandler();
			}

			if (path.StartsWith("/!") || path.StartsWith("/~") || path.StartsWith("/-")){
				path = "/" + path.Substring(2);
			}
			
			if (path == "/wiki"){
				return new WikiHandler();
			}
			if (path.SmartSplit().Count >= 2 || path.EndsWith(".qweb")){
				return new HostQwebHandler();
			}
			return new NotFoundHandler();
		}

		/// <summary>
		///     Регистрирует хэндлер для определенного адреса
		/// </summary>
		/// <param name="path"></param>
		/// <param name="handler"></param>
		public void Register(string path, IRequestHandler handler){
			lock (this){
				_cache[path] = handler;
			}
		}
	}
}