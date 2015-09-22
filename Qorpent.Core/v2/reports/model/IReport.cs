using System.Collections.Generic;
using System.Security.Principal;
using qorpent.v2.model;
using qorpent.v2.reports.core;
using Qorpent.Model;

namespace qorpent.v2.reports.model {
    public interface IReport :IItem,IWithRole,IWithDefinition,IWithIndex{
        IList<IReportAgentDefinition> Agents { get; set; }
        IDictionary<string, ReportParameter> Parameters { get; set; }
    
    }
}