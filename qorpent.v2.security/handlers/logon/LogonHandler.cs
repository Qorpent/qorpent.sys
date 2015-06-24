using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "handler.logon", ServiceType = typeof (ILogonHandler))]
    public class LogonHandler : ILogonHandler {
        
       
        [Inject]
        public ILogonService LogonService { get; set; }

        [Inject]
        public IHttpTokenService TokenService { get; set; }

        [Inject]
        public ILoggyManager LoggyManager { get; set; }
        private ILoggy _userOpLog;
        private const string UserOpName = "user.op.secure.logon";
        private ILoggy UserOpLog {
            get { return _userOpLog ?? (_userOpLog = (this.LoggyManager ?? Loggy.Manager).Get(UserOpName)); }
            set { _userOpLog = value; }
        }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var ctx = RequestParameters.Create(context);
            var login = ctx.Get("login");
            var password = ctx.Get("pass");
            var identity = (Identity) LogonService.Logon(login, password);
            context.User = new GenericPrincipal(identity, null);
            if (identity.IsAuthenticated && !identity.IsGuest) {
                var token = TokenService.Create(context.Request);
                TokenService.Store(context.Response, context.Request.Uri, token);
                if (UserOpLog.IsForInfo()) {
                    UserOpLog.Info(new {
                        auth=true,
                        name=identity.Name,
                        type=identity.AuthenticationType,
                        isadmin=identity.IsAdmin
                    }.stringify());
                }
                context.Finish("true");
            }
            else {
                TokenService.Store(context.Response, context.Request.Uri, null);
                if (UserOpLog.IsForWarn())
                {
                    UserOpLog.Warn(new
                    {
                        auth = false,
                        login,
                        error = identity.Error
                    }.stringify());
                }
                context.Finish("false");
            }
        }
    }
}