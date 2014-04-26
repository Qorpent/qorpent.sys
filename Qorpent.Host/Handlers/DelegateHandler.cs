using System;
using System.Net;
using System.Threading;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class DelegateHandler :IRequestHandler
	{
		private Action<IHostServer, HttpListenerContext, string, CancellationToken> _handler;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="handler"></param>
		public DelegateHandler(Action<IHostServer, HttpListenerContext, string, CancellationToken> handler)
		{
			this._handler = handler;
		}

		/// <summary>
		/// Выполняет указанный запрос
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel)
		{
			_handler(server, callcontext, callbackEndPoint, cancel);
		}
	}
}