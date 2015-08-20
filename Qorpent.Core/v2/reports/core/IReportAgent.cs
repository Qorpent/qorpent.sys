using System.Threading.Tasks;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.Model;

namespace qorpent.v2.reports.core {
    public interface IReportAgent:IWithStringId,IWithIndex {
        ReportPhase Phase { get; set; }
        bool Parallel { get; set; }
        bool IsMatch(IReportContext context);
        void Initialize(IReportAgentDefinition definition );
        Task Execute(IReportContext context, IScope scope = null);
    }
}