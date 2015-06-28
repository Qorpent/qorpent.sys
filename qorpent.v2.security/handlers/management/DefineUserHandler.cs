using System.Threading;
using qorpent.v2.security.management;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "defineuser.handler", ServiceType = typeof (IDefineUserHandler))]
    [UserOp("defusr",Secure = true,SuccessLevel = LogLevel.Warn,ExceptionLevel = LogLevel.Error)]
    public class DefineUserHandler : UserOperation, IDefineUserHandler {
        [Inject]
        public IUpdateUserProcessor UserProcessor { get; set; }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var result = UserProcessor.DefineUser(context);
            var param = RequestParameters.Create(context);
            return new HandlerResult {
                Result = result,
                Data = new{
                    result,
                    call = new {
                        q = param.Query,
                        j = param.Json.jsonify()
                    }
                }
            };
        }

    }
}