using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "handler.logon", ServiceType = typeof (ILogonHandler))]
    [UserOp("logon",Secure =true,SuccessLevel = LogLevel.Info,ErrorLevel = LogLevel.Warn,TreatFalseAsError = true)]
    public class LogonHandler : UserOperation,ILogonHandler {
        
       
        [Inject]
        public ILogonService LogonService { get; set; }

        [Inject]
        public IHttpTokenService TokenService { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="context"></param>
        /// <param name="callbackEndPoint"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var ctx = RequestParameters.Create(context);
            var login = ctx.Get("login");
            var password = ctx.Get("pass");
            var identity = (Identity)LogonService.Logon(login, password);
            context.User = new GenericPrincipal(identity, null);
            if (identity.IsAuthenticated && !identity.IsGuest)
            {
                var token = TokenService.Create(context.Request);
                TokenService.Store(context.Response, context.Request.Uri, token);
                return new HandlerResult {Result = true, Data = identity};
            }
            TokenService.Store(context.Response, context.Request.Uri, null);
            return new HandlerResult {Result = false, Data = identity};
        }

        public override string GetUserOperationLog(bool iserror, LogLevel level, HandlerResult result) {
            var identity = result.Data as Identity;
            if (iserror) {
                return new {
                    auth = false,
                    login = identity.Name,
                    error = identity.Error
                }.stringify();
            }
            return new {
                auth = true,
                name = identity.Name,
                type = identity.AuthenticationType,
                isadmin = identity.IsAdmin
            }.stringify();
        }

    }
}