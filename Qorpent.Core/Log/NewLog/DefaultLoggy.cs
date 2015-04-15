using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qorpent.Log.NewLog {
    public class DefaultLoggy : ILoggy {
        private readonly IList<ILogAppender> _appenders =new List<ILogAppender>();
        private IList<ILoggy> _subLoggers;

        public void Dispose() {
            foreach (var appender in Appenders) {
                try {
                    appender.Dispose();
                }
                catch (Exception ex) {
                    
                }
            }
        }

        public void Write(LoggyMessage message) {
            if (IsFor(message.Level)) {
                if (string.IsNullOrWhiteSpace(message.LoggerName)) {
                    message.LoggerName = this.Name;
                }
                foreach (var appender in Appenders) {
                    if (appender.Level <= message.Level) {
                        appender.Write(message);
                    }
                }
                if (HasSubloggers) {
                    foreach (var subLogger in SubLoggers) {
                        if (subLogger.IsFor(message.Level)) {
                            subLogger.Write(message);
                        }
                    }
                }
            }
        }

        public void Flush() {
            var tasks = new List<Task>();
            foreach (var appender in Appenders) {
                tasks.Add(Task.Run(()=>appender.Flush()));
            }
            if (HasSubloggers) {
                foreach (var subLogger in SubLoggers) {
                    tasks.Add(Task.Run(()=>subLogger.Flush()));
                }
            }
            Task.WaitAll(tasks.ToArray(), 1000);
        }

        public LogLevel Level { get; set; }

        public string Name { get; set; }

        public bool IsFor(LogLevel level) {
            if (level < Level) return false;
            var hasappender = Appenders.Any(_ => level >= _.Level);
            var hassubloggers = HasSubloggers && SubLoggers.Any(_ => _.IsFor(level));
            return hasappender || hassubloggers;
        }

        public IList<ILogAppender> Appenders {
            get { return _appenders; }
        }



        public bool Isolated { get; set; }

        public bool HasSubloggers {
            get { return !Isolated && null != _subLoggers && 0 != _subLoggers.Count; }
        }
        public IList<ILoggy> SubLoggers {
            get { return _subLoggers ?? (_subLoggers = new List<ILoggy>()); }
        }
    }
}