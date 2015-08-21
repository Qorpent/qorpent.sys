using Qorpent;
using Qorpent.Host;
using Qorpent.IoC;

namespace qorpent.v2.reports.api.restful {
    [ContainerComponent(Lifestyle.Transient, Name = "qorpent.report.handler.init", ServiceType = typeof(IHostServerInitializer))]
    public class ReportHandlerInstaller : ServiceBase,IHostServerInitializer {
        public void Initialize(IHostServer server) {
            server.Factory.Register("/report",ResolveService<IRequestHandler>("qorpent.report.handler"));
        }
    }
}