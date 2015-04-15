namespace Qorpent.Log.NewLog {
    public static class LoggyExtensions {
        public static void Debug(this ILoggy loggy, params object[] args) {
            Write(loggy,LogLevel.Debug,(object[])args);
        }
        public static void Trace(this ILoggy loggy, params object[] args)
        {
            Write(loggy, LogLevel.Trace, (object[])args);
        }
        public static void Info(this ILoggy loggy, params object[] args)
        {
            Write(loggy, LogLevel.Info, (object[])args);
        }
        public static void Warn(this ILoggy loggy, params object[] args)
        {
            Write(loggy, LogLevel.Warning, (object[])args);
        }
        public static void Error(this ILoggy loggy, params object[] args)
        {
            Write(loggy, LogLevel.Error, (object[])args);
        }
        public static void Fatal(this ILoggy loggy, params object[] args)
        {
            Write(loggy, LogLevel.Fatal, (object[])args);
        }

        public static void Write(this ILoggy loggy, LogLevel level, params object[] args) {
            if (loggy.IsFor( level)) {
                var message = new LoggyMessage(level, args);
                loggy.Write(message);
            }
        }

        public static bool IsForDebug(this ILoggy loggy) {
            return loggy.IsFor(LogLevel.Debug);
        }
        public static bool IsForTrace(this ILoggy loggy)
        {
            return loggy.IsFor( LogLevel.Trace);
        }
        public static bool IsForInfo(this ILoggy loggy)
        {
            return loggy.IsFor( LogLevel.Info);
        }
        public static bool IsForWarn(this ILoggy loggy)
        {
            return loggy.IsFor( LogLevel.Warning);
        }
        public static bool IsForError(this ILoggy loggy)
        {
            return loggy.IsFor( LogLevel.Error);
        }
        public static bool IsForFatal(this ILoggy loggy)
        {
            return loggy.IsFor( LogLevel.Fatal);
        }
        
    }
}