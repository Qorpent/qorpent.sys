using System.Linq;
using System.Security;
using System.Threading;
using qorpent.Security;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "userlist.handler", ServiceType = typeof(IUserListHandler))]
    [UserOp("usrlist", Secure = true, SuccessLevel = LogLevel.Trace, ExceptionLevel = LogLevel.Error)]
    public class UserListHandler : UserOperation, IUserListHandler {
        [Inject]
        public IUserService Users { get; set; }

        [Inject] public IRoleResolverService Roles { get; set; }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            if (
                !Roles.IsInRole(context.User.Identity, $"{SecurityConst.ROLE_SECURITY_ADMIN},{SecurityConst.ROLE_DOMAIN_ADMIN}")) {
                throw new SecurityException("only admins allowed to call this");
            }
            var j = RequestParameters.Create(context).AsJson();
            var uq = new UserSearchQuery();
            uq.LoadFromJson(j);
            //check if not total admin
            if (!Roles.IsInRole(context.User.Identity, $"{SecurityConst.ROLE_SECURITY_ADMIN}")) {
                var domain = ((Identity) context.User.Identity).User.Domain;
                if (string.IsNullOrWhiteSpace(domain)) {
                    throw new SecurityException("invalid domain for user "+domain);
                }
                if (!string.IsNullOrWhiteSpace(uq.Domain) && uq.Domain != domain) {
                    throw new SecurityException("try access not self domain "+domain+" ("+uq.Domain+")");
                }
                uq.Domain = domain;
            }
            var users = Users.SearchUsers(uq).ToArray();
            return new HandlerResult {
                Result = new {items= users}
            };
        }
    }
}