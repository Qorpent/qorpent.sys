using System.Threading.Tasks;
using qorpent.v2.console;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.reports.api.console {
    [ContainerComponent(Lifestyle.Singleton, Name = "qorpent.report.command", ServiceType = typeof(IConsoleCommand))]
    public class ReportCommand : ConsoleCommandBase
    {
        [Inject]
        public IReportService Reports { get; set; }
        protected override async Task InternalExecute(IConsoleContext context, ConsoleCommandResult result, string commandname, string commandstring, IScope scope)
        {
            var request = ResolveService<IReportRequest>("", context);
            await Reports.Execute(request);
        }
        
    }
}