using qorpent.v2.reports.storage;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Mvc.Binding;

namespace qorpent.v2.reports.api.restful
{
    [Action("reports.list")]
    public class ReportListAction :ActionBase
    {
        [Inject]
        public IReportProvider ReportProvider { get; set; }
        [Bind]
        public string Query { get; set; }
        protected override object MainProcess() {
            return ReportProvider.Search(Query);
        }
    }
}
