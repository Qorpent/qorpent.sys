using System;
using System.Collections.Concurrent;

namespace Qorpent.Core.Tests.Log.NewLogs {
    public interface ILoggyManager {
        ConcurrentDictionary<string, ILoggy> Loggers { get; }
        ILoggy Get(string name = null, Action<ILoggy> setup = null);
    }
}