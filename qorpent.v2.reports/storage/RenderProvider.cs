using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.reports.storage {
    [ContainerComponent(ServiceType = typeof(IRenderProvider),Lifestyle=Lifestyle.Singleton, Name="qorpent.reports.template.provider")]
    public class RenderProvider : ExtensibleServiceBase<IRenderSource>, IRenderProvider {
        public IReportRender GetRender(string uri, IScope scope) {
            foreach (var source in Extensions) {
                var template = source.GetRender(uri, scope);
                if (null != template) {
                    template.Initialize(uri, scope);
                    return template;
                }
            }
            return null;
        }
    }
}