using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qorpent.v2.reports.storage;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Mvc.Binding;

namespace qorpent.v2.reports.restful
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
