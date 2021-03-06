using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using qorpent.v2.console;
using qorpent.v2.reports.model;
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
        IIdentity User { get; set; }
        bool Standalone { get; set; }
        bool NoFinalizeOnError { get; set; }
        IDictionary<string, object> Parameters { get; set; }
        IReport PreparedReport { get; set; }
        Stream Stream { get; set; }
        void Initialize();
    }
}