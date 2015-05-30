using System.Threading;
using qorpent.v2.security.authorization;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.handlers {
    [ContainerComponent(Lifestyle.Singleton, "isrole.handler", ServiceType = typeof(IIsInRoleHandler))]
    public class IsInRoleHandler : IIsInRoleHandler
    {

        [Inject]
        public IRoleResolverService Roles { get; set; }
        
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var ctx = RequestParameters.Create(context);
            var login = ctx.Get("login");
            var role = ctx.Get("role");
            var exact = ctx.Get("exact").ToBool();
            if (string.IsNullOrWhiteSpace(role)) {
                context.Finish("{\"error\":\"emptyrole\"}",status:500);
                return;
            }
            if (string.IsNullOrWhiteSpace(login)) {
                login = context.User.Identity.Name;
            }
            bool result = false;
            if (login != context.User.Identity.Name) {
                if (!Roles.IsInRole(context.User.Identity, "ADMIN")) {
                    context.Finish("{\"error\":\"adminrequire\"}", status: 500);
                    return;
                }
                result = Roles.IsInRole(login, role, exact);
            }
            else {
                result = Roles.IsInRole(context.User.Identity, role, exact);
            }
            context.Finish(result.ToString().ToLowerInvariant());
        }
    }
}