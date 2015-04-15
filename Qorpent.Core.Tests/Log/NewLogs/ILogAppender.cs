using System;
using Qorpent.Log;

namespace Qorpent.Core.Tests.Log.NewLogs {
    public interface ILogAppender:IDisposable {
        void Write(LoggyMessage message);
        void Flush();
        LogLevel Level { get; set; }
    }
}