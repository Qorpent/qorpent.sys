using System;
using System.Collections.Generic;

namespace Qorpent.Log.NewLog {
    public static class Loggy {

        private static ILoggyManager _manager;

        public static ILoggyManager Manager {
            get {
                if (null == _manager) {
                    Manager = new LoggyManager();        
                }
                return _manager;
                ;
            }
            set {
                if (_manager != value && value != null) {
                    _manager = value;
                    OnChangeManager?.Invoke(_manager);
                }
            }
        }

        public static Action<ILoggyManager> OnChangeManager;

        public static ILoggy Default {
            get { return Manager.Get(); }
        }

        public static ILoggy Get(string name = null, Action<ILoggy> setup = null) {
            return Manager.Get(name, setup);
        }
 

        public static LogLevel Level {
            get { return Default.Level; }
            set { Default.Level = value; }
        }

        public static  IList<ILogAppender> Appenders {
            get { return Default.Appenders; }
        }
        public static void Debug(params object[] test) {
            Write(LogLevel.Debug,(object[])test);
        }

        public static bool IsForDebug() {
            return IsFor(LogLevel.Debug);
        }
        public static bool IsForTrace()
        {
            return IsFor(LogLevel.Trace);
        }
        public static bool IsForInfo()
        {
            return IsFor(LogLevel.Info);
        }
        public static bool IsForWarn()
        {
            return IsFor(LogLevel.Warn);
        }
        public static bool IsForError()
        {
            return IsFor(LogLevel.Error);
        }
       
        public static bool IsFor(LogLevel level) {
            return Default.IsFor(level);
        }

        public static void Trace(params object[] test)
        {
            Write(LogLevel.Trace, (object[])test);
        }

        public static void Info(params object[] test)
        {
            Write(LogLevel.Info, (object[])test);
        }

        public static void Warn(params object[] test)
        {
            Write(LogLevel.Warn, (object[])test);
        }

        public static void Error(params object[] test)
        {
            Write(LogLevel.Error, (object[])test);
        }

        public static void Fatal(params object[] test)
        {
            Write(LogLevel.Fatal, (object[])test);
        }

        public static void Write(LogLevel level, params object[] args) {
            Default.Write(level,(object[])args);
        }

        public static void Write(LoggyMessage message) {
            Default.Write(message);
        }

        public static void Flush() {
            Default.Flush();
        }
    }
}