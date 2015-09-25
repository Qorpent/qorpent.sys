using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers {
    public class DropStaticCache : RequestHandlerBase {
        public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            server.Static.DropCache();
        }
    }
}