using System;
using System.Net;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class DelegateHandler :RequestHandlerBase
	{
		private Action<IHostServer, WebContext, string, CancellationToken> _handler;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="handler"></param>
        public DelegateHandler(Action<IHostServer, WebContext, string, CancellationToken> handler)
		{
			this._handler = handler;
		}


	    public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
	        CancellationToken cancel) {
                _handler(server, context, callbackEndPoint, cancel);
	    }
	}
}