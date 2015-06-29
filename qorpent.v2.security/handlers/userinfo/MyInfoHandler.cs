using System;
using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers.userinfo {
    [ContainerComponent(Lifestyle.Singleton, "myinfo.handler", ServiceType = typeof (IMyInfoHandler))]
    public class MyInfoHandler : IMyInfoHandler {
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            if (!context.User.Identity.IsAuthenticated) {
                context.Finish("{\"notauth\":true}");
            }
            bool fullinfo = context.Request.Uri.Query.Contains("full");
            if (fullinfo) {
                CheckAllowFull(context.User.Identity);
            }
            context.Finish(context.User.Identity.stringify(fullinfo?"admin":"ui"));
        }

        private void CheckAllowFull(IIdentity identity) {
            var i = identity as Identity;
            if(null==i)throw new Exception("only QH Identity supported");
            if (i.IsAdmin) return;
            if(null!=i.ImpersonationSource)return;
            throw new Exception("full info not allowed");
        }
    }
}