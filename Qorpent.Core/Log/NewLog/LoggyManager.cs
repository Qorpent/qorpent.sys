using System;
using System.Collections.Concurrent;

namespace Qorpent.Log.NewLog {
    public class LoggyManager:ILoggyManager {
        readonly ConcurrentDictionary<string, ILoggy> loggers = new ConcurrentDictionary<string, ILoggy>();

        public ConcurrentDictionary<string, ILoggy> Loggers {
            get { return loggers; }
        }

        public ILoggy Get(string name = null, Action<ILoggy> setup= null) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = "default";
            }
            var result = loggers.GetOrAdd(name, n => {
                var l = new DefaultLoggy {Name = n};
                if (n != "default") {
                    l.SubLoggers.Add(Get());
                    l.Level = Get().Level;
                }
                else {
                    l.Isolated = true;
                }

                if (null != setup) {
                    setup(l);
                }
                
                return l;
            });
            return result;
        }
    }
}