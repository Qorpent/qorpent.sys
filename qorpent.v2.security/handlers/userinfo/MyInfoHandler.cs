using System.Threading;
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

            context.Finish(context.User.Identity.stringify());
        }
    }
}