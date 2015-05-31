using System.Threading;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "isauth.handler", ServiceType = typeof (IIsAuthHandler))]
    public class IsAuthHandler : IIsAuthHandler {
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            context.Finish(context.User.Identity.IsAuthenticated.ToString().ToLowerInvariant());
        }
    }
}