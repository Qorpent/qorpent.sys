using System;
using System.Collections.Concurrent;

namespace Qorpent.Log.NewLog {
    public interface ILoggyManager {
        ConcurrentDictionary<string, ILoggy> Loggers { get; }
        ILoggy Get(string name = null, Action<ILoggy> setup = null);
    }
}