using System.Net;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host {
    public abstract class RequestHandlerBase : IRequestHandler {
        public abstract void Run(IHostServer server, WebContext context,
            string callbackEndPoint,
            CancellationToken cancel);
    }
}