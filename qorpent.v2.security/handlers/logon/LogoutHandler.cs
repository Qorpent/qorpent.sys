using System.Threading;
using qorpent.v2.security.authentication;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.logon {
    
    [ContainerComponent(Lifestyle.Singleton, "logout.handler", ServiceType = typeof (ILogoutHandler))]
    [UserOp("logout",SuccessLevel = LogLevel.Info,Secure = true)]
    public class LogoutHandler :UserOperation, ILogoutHandler {
        [Inject]
        public IHttpTokenService TokenService { get; set; }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            TokenService.Store(context.Response, context.Request.Uri, null);
            return new HandlerResult {Result = true, Data = context.User.Identity };
        }

        public override string GetUserOperationLog(bool iserror, LogLevel level, HandlerResult result,WebContext context) {
            return new {logout = (result.Data as Identity).Name}.stringify();
        }
    }
}