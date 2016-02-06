using qorpent.v2.model;
using qorpent.v2.reports.core;
using Qorpent.Model;

namespace qorpent.v2.reports.model {
    public interface IReportAgentDefinition : IItem, IWithIndex , IWithDefinition
    {
        ReportPhase Phase { get; set; }
        object Definition { get; set; }
        int Idx { get; set; }
        bool Parallel { get; set; }
        IReportAgent Instance { get; set; }
    }
}