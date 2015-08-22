using qorpent.v2.reports.core;
using Qorpent;

namespace qorpent.v2.reports.agents {
    public interface IReportRender {
        void Initialize(string uri, IScope scope);
        IScope Render(IReportContext context, IScope scope, object item);
    }
}