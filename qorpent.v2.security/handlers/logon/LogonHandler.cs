using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "logon.handler", ServiceType = typeof (ILogonHandler))]
    public class LogonHandler : ILogonHandler {
        [Inject]
        public ILogonService LogonService { get; set; }

        [Inject]
        public IHttpTokenService TokenService { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var ctx = RequestParameters.Create(context);
            var login = ctx.Get("login");
            var password = ctx.Get("pass");
            var identity = (Identity) LogonService.Logon(login, password);
            context.User = new GenericPrincipal(identity, null);
            if (identity.IsAuthenticated && !identity.IsGuest) {
                var token = TokenService.Create(context.Request);
                TokenService.Store(context.Response, context.Request.Uri, token);
                context.Finish("true");
            }
            else {
                TokenService.Store(context.Response, context.Request.Uri, null);
                context.Finish("false");
            }
        }
    }
}