using System.Net;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host {
    public abstract class RequestHandlerBase : IRequestHandler {
        public virtual void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel) {
            Run(server, callcontext.Request, callcontext.Response, callbackEndPoint, cancel);
        }

        public abstract void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response,
            string callbackEndPoint,
            CancellationToken cancel);
    }
}