using System;
using System.Linq;
using System.Runtime.InteropServices;
using qorpent.v2.reports.model;
using qorpent.v2.reports.storage;
using qorpent.v2.security.authorization;
using Qorpent;
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
        [Inject]
        public IRoleResolverService Roles { get; set; }
        protected override object MainProcess() {
            var result = ReportProvider.Search(Query);
            var items = result.Items.OfType<IReport>().Where(_=>Roles.IsInRole(User.Identity, _.Role)).ToArray();
            result.TypedItems = null;
            result.Items = items;
            return result.stringify(Mode);
        }

   
    }
}
