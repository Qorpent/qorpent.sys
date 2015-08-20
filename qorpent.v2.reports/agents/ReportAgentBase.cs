using System.Threading.Tasks;
using qorpent.v2.reports.core;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.Model;

namespace qorpent.v2.reports.agents {
    public abstract class ReportAgentBase : ServiceBase, IReportAgent,IWithDefinition {
        public string Id { get; set; }
        public int Idx { get; set; }
        public ReportPhase Phase { get; set; }
        public bool Parallel { get; set; }

        public bool IsMatch(IReportContext context) {
            if (context.Request.Flags.Contains("-" + Id)) return false;
            if (context.Request.Flags.Contains("explicit")) {
                if (!context.Request.Flags.Contains(Id)) return false;
            }
            return true;
        }

        public virtual void Initialize(IReportAgentDefinition definition) {
            if (definition.Phase != ReportPhase.None) {
                Phase = definition.Phase;
            }
            if (0 != definition.Idx) {
                Idx = definition.Idx;
            }
            if (definition.Parallel) {
                this.Parallel = definition.Parallel;
            }
            Id = definition.Id;
            Definition = definition;
        }

        public abstract Task Execute(IReportContext context,IScope scope = null);

        public object Definition { get; set; }
    }
}