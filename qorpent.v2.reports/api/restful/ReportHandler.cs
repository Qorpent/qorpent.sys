using System.Threading;
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
            var waiter = Reports.Execute(request);
            waiter.Wait();
        }
    }
}