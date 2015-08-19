using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using qorpent.v2.console;
using qorpent.v2.reports.storage;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.api.console
{
    [ContainerComponent(Lifestyle.Singleton,ServiceType = typeof(IConsoleCommand),Name = "qorpent.reports.list.command")]
    public class ReportListCommand : ConsoleCommandBase
    {
        [Inject]
        public IReportProvider ReportProvider { get; set; }
       
        protected override async Task InternalExecute(IConsoleContext context, ConsoleCommandResult result, string commandname, string commandstring, IScope scope) {
            var data = ReportProvider.Search(commandstring);
            var pretty = context.Parameters.Get("pretty").ToBool();
            context.WriteLine(data.stringify(pretty: pretty));
        }
    }
}
