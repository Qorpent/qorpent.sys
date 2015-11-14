using System;
using System.Threading;
using qorpent.v2.security.management;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "initlient.handler", ServiceType = typeof(IInitClientHandler))]
    [UserOp("initclient", Secure = true, SuccessLevel = LogLevel.Warn, ExceptionLevel = LogLevel.Error)]
    public class InitClientHandler : UserOperation, IInitClientHandler
    {
        [Inject]
        public IClientService Clients { get; set; }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var j = RequestParameters.Create(context).Json;
            if (null == j) {
                throw new Exception("invalid json");
            }
            var ir = new InitClientRecord();
            ir.LoadFromJson(j);
            var result = Clients.Init(context.User.Identity,ir);
            return new HandlerResult
            {
                Result = result,
                Data = new
                {
                    result,
                    call = new
                    {
                        ir 
                    }
                }
            };
        }

    }
}