using System;
using System.Collections.Generic;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.Log.NewLog;

namespace qorpent.v2.reports.core {
    public interface IReportContext {
        IScope Scope { get; set; }
        IReport Report { get; set; }
        IReportRequest Request { get; set; }
        IList<IReportAgent> Agents { get; }
        ILoggy Log { get; set; }
        Exception Error { get; set; }
        void SetHeader(string name, string value);
        void Write(object data);
        void Finish(Exception error = null);
    }
}