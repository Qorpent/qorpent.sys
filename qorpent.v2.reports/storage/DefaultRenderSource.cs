using System;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.reports.storage {
    [ContainerComponent(Lifestyle.Singleton,ServiceType = typeof(IRenderSource),Name="qorpent.reports.template.source.default")]
    public class DefaultRenderSource : ServiceBase, IRenderSource {
        public IReportRender GetRender(string uri, IScope scope) {
            var u = new Uri(uri);
            return ResolveService<IReportRender>("qorpent.reports." + u.Scheme + ".template");
        }
    }
}