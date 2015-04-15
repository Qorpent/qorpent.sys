using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log.NewLogs {
    public interface ILoggy : ILogAppender
    {
        string Name { get; set; }
        bool IsFor(LogLevel level);
        IList<ILogAppender> Appenders { get; }
        bool Isolated { get; set; }
        IList<ILoggy> SubLoggers { get; }
    }
}