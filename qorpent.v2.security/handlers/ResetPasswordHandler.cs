using System.Threading;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers {
    [ContainerComponent(Lifestyle.Singleton, "resetpwd.handler", ServiceType = typeof(IResetPasswordHandler))]
    public class ResetPasswordHandler : IResetPasswordHandler {
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            throw new System.NotImplementedException();
        }
    }
}