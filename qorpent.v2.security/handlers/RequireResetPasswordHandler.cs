using System.Threading;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers {
    [ContainerComponent(Lifestyle.Singleton, "resetpwdreq.handler", ServiceType = typeof(IRequireResetPasswordHandler))]
    public class RequireResetPasswordHandler : IRequireResetPasswordHandler
    {
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel)
        {
            throw new System.NotImplementedException();
        }
    }
}