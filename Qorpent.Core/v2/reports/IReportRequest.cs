using System.Collections.Generic;
using qorpent.v2.console;
using Qorpent.IO.Http;

namespace qorpent.v2.reports {
    public interface IReportRequest {
        string Id { get; set; }
        bool DataOnly { get; set; }
        string Format { get; set; }
        object Query { get; set; }
        object Json { get; set; }
        IList<string> Flags { get; set; }
        WebContext WebContext { get; set; }
        IConsoleContext ConsoleContext { get; set; }
        void Initialize();
    }
}