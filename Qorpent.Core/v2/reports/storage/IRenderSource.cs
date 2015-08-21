using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;

namespace qorpent.v2.reports.storage {
    public interface IRenderSource {
        IReportRender GetRender(string uri,IScope scope);
    }
}