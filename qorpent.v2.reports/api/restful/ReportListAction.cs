using qorpent.v2.reports.storage;
using Qorpent.Experiments;
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
        [Bind]
        public string Mode { get; set; }
        protected override object MainProcess() {
            var result = ReportProvider.Search(Query);
            return result.stringify(Mode);
        }
    }
}
