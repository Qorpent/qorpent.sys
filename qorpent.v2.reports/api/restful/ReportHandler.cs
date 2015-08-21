using System;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.reports.api.restful {
    [ContainerComponent(Lifestyle.Singleton,Name="qorpent.report.handler",ServiceType = typeof(IRequestHandler))]
    public class ReportHandler :  RequestHandlerBase {
        [Inject]
        public IReportService Reports { get; set; }

        public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var request = ResolveService<IReportRequest>("",context);
            request.NoFinalizeOnError = true;
            var waiter = Reports.Execute(request);
            waiter.Wait();
            var result = waiter.Result;
            if (null != result.Error) {
                context.Finish(GetErrorJson(result.Error).stringify(),status:500);
            }
        }

        object GetErrorJson(Exception e) {
            if (null != e) {
                return new {
                    type = e.GetType().Name,
                    message = e.Message,
                    stack = e.StackTrace,
                    inner = GetErrorJson(e.InnerException)
                };
            }
            return null;
        }
    }
}