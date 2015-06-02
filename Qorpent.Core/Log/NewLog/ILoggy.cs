using System.Collections.Generic;

namespace Qorpent.Log.NewLog {
    public interface ILoggy : ILogAppender
    {
        string Name { get; set; }
        bool IsFor(LogLevel level);
        IList<ILogAppender> Appenders { get; }
        bool Isolated { get; set; }
        IList<ILoggy> SubLoggers { get; }
    }
}