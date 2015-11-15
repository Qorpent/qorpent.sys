using System;
using System.Threading;
using qorpent.v2.security.management;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "client.handler", ServiceType = typeof(IClientHandler))]
    [UserOp("initclient", Secure = true, SuccessLevel = LogLevel.Warn, ExceptionLevel = LogLevel.Error)]
    public class ClientHandler : UserOperation, IClientHandler
    {
        [Inject]
        public IClientService Clients { get; set; }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var j = RequestParameters.Create(context).Json;
            if (null == j) {
                throw new Exception("invalid json");
            }
            var ir = new ClientRecord();
            ir.LoadFromJson(j);

            var result = GetResult(context, ir);


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

        private ClientResult GetResult(WebContext context, ClientRecord ir) {
            var id = context.User.Identity;
            if (string.IsNullOrWhiteSpace(ir.Operation) || "init" == ir.Operation) {
                return Clients.Init(id, ir);
            }
            if ("towork" == ir.Operation) {
                return Clients.ToWork(id, ir.SysName);
            }
            if ("todemo" == ir.Operation)
            {
                return Clients.ToDemo(id, ir.SysName);
            }
            if ("setexpire" == ir.Operation) {
                return Clients.SetExpire(id, ir.SysName, ir.Expire);
            }
            throw new Exception("invalid operation "+ir.Operation );
        }
    }
}