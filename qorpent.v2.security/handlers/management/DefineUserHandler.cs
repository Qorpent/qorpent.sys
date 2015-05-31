using System.Threading;
using qorpent.v2.security.management;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "defineuser.handler", ServiceType = typeof (IDefineUserHandler))]
    public class DefineUserHandler : IDefineUserHandler {
        [Inject]
        public IUpdateUserProcessor UserProcessor { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var result = UserProcessor.DefineUser(context);
            if (!result.IsError) {
                context.Finish(result.ResultUser.stringify());
            }
            else {
                context.Finish(result.stringify(), status: 500);
            }
        }
    }
}