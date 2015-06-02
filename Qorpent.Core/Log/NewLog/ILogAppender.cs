using System;

namespace Qorpent.Log.NewLog {
    public interface ILogAppender:IDisposable {
        void Write(LoggyMessage message);
        void Flush();
        LogLevel Level { get; set; }
    }
    
}