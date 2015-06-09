using System;
using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers.logon {
    public interface IIsAuthHandler : IRequestHandler {
    }

    public interface IImpersonateHandler : IRequestHandler
    {
    }
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(Lifestyle.Singleton,"handler.impersonate")]
    public class ImpersonateHandler : IImpersonateHandler {
        [Inject]
        public IUserService Users { get; set; }

        [Inject]
        public IHttpTokenService TokenService { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var id = context.User.Identity as Identity;
            if(null==id)throw new Exception("invalid identity type");
            if (null == id.ImpersonationSource) {
                if(!id.IsAdmin)throw new Exception("not admin");
            }
            var src = id.ImpersonationSource ?? id;
            var p = RequestParameters.Create(context);
            var to = p.Get("login");
            Identity newid = null;
            if (string.IsNullOrWhiteSpace(to)) {
                newid = (Identity)src;
            }
            else {
                var user = Users.GetUser(to);
                if (null != user) {
                    newid = new Identity(user);

                }
                else {
                    newid =new Identity{Name = to, IsAuthenticated = true};
                }
                newid.ImpersonationSource = src;
            }
            context.User = new GenericPrincipal(newid,null);
            var token = TokenService.Create(context.Request);
            newid.Token = token;
            TokenService.Store(context.Response,context.Request.Uri,token);
            context.Finish(newid.stringify());
        }
    }
}