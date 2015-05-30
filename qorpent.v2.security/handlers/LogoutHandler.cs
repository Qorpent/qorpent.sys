using System.Threading;
using qorpent.v2.security.authentication;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers {
    [ContainerComponent(Lifestyle.Singleton, "logout.handler", ServiceType = typeof(ILogoutHandler))]
    public class LogoutHandler : ILogoutHandler
    {
        [Inject]
        public IHttpTokenService TokenService { get; set; }
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel)
        {
            TokenService.Store(context.Response,context.Request.Uri,null);
            context.Finish("true");
        }
    }
}