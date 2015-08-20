using System;

namespace qorpent.v2.reports.core {
    [Flags]
    public enum ReportPhase {
        None = 0,
        Init = 1,
        Data = 1<<1,
        Prepare = 1<<2, 
        Render = 1<<3,
        Finalize =1<<4
    }
}