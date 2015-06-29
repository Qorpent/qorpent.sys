using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IO.Http;

namespace qorpent.v2.security.handlers {
    public class HandlerBase : IRequestHandler {
        public virtual void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
            DefaultProcess(server, context, callbackEndPoint, cancel);
            
        }

        protected virtual HandlerResult DefaultProcess(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
            var result = GetResult(server, context, callbackEndPoint, cancel) ?? HandlerResult.Null;
            var outer = result.Result;
            if (result.Mime == "application/json") {
                var str = outer as string;
                if (null != str &&
                    ((str.StartsWith("{") && str.EndsWith("}")) || (str.StartsWith("[") && str.EndsWith("]")))) {
                    outer = str.jsonify();
                }
                outer = outer.stringify();
                context.Finish(outer, result.Mime, result.State);
            }
            return result;
        }

        protected virtual HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
            return null;
        }
    }
}