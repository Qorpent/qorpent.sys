using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log.NewLog {
    [ContainerComponent(Lifestyle.Singleton,"loggy.manager",ServiceType=typeof(ILoggyManager))]
    public class LoggyManager: ILoggyManager,IContainerBound {
        readonly ConcurrentDictionary<string, ILoggy> loggers = new ConcurrentDictionary<string, ILoggy>();

        public ConcurrentDictionary<string, ILoggy> Loggers {
            get { return loggers; }
        }

        public LoggyManager() {
            Appenders =  new Dictionary<string, ILogAppender>();
        }

        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        public IDictionary<string, ILogAppender> Appenders { get; set; } 

        public void InitializeFromXml(XElement e) {
           
            if (e.Attribute("overrideloggy").ToBool()) {
                Loggy.Manager = this;
            }

            IList<LoggyMessage> selflog = new List<LoggyMessage>();
            foreach (var a in e.Elements("appender")) {
                var name = a.Attr("code");
                var ap = ReadAppender(selflog, a);
                if (null != ap) {
                    Appenders[name] = ap;
                }
            }
            bool wasdefault = false;
            foreach (var l in e.Elements("logger")) {
                var name = l.Attr("code","default");
                if (name == "default") {
                    Get(name, _ => {
                        PrepareLogger(_, l, selflog);
                        if (0 == _.Appenders.Count) {
                            foreach (var appender in Appenders.Values) {
                                _.Appenders.Add(appender);
                            }
                        }
                    });
                }
            }
            foreach (var l in e.Elements("logger")) {
                var name = l.Attr("code","default");
                if (name != "default") {
                    Get(name, _ => {
                        PrepareLogger(_, l, selflog);
                    });
                }
            }
            var logg = Get("loggy.manager");
           if (0 != selflog.Count) {
                foreach (var logMessage in selflog) {
                    logg.Write(logMessage);
                }
            }
            logg.Info(new{logmanager="initialized"}.stringify());
        }

        private void PrepareLogger(ILoggy _, XElement l, IList<LoggyMessage> selflog)
        {
            _.Isolated = l.Attr("isolated").ToBool();
            _.Level = l.Attr("level", "Info").To<LogLevel>();
            foreach (var element in l.Elements("appender")) {
                var _ref = element.Attr("ref");
                if (!string.IsNullOrWhiteSpace(_ref)) {
                    if (Appenders.ContainsKey(_ref)) {
                        _.Appenders.Add(Appenders[_ref]);
                    }
                    else {
                        selflog.Add(new LoggyMessage (LogLevel.Error,  "invalid appender ref" ));
                    }
                }
                else {
                    var ap = ReadAppender(selflog, element);
                    if (null != ap) {
                        _.Appenders.Add(ap);
                    }
                }
            }
        }

        private ILogAppender ReadAppender(IList<LoggyMessage> selflog, XElement a)
        {
            var name = a.Attr("code");
            if (string.IsNullOrWhiteSpace(name)) {
                selflog.Add(new LoggyMessage (LogLevel.Error, "not named appender not allowed" ));
                return null;
            }
            var type = a.Attr("type");
            if (string.IsNullOrWhiteSpace(type)) {
                type = name;
            }
            ILogAppender ap = null;
            if (type == "console") {
                ap = BuildConsoleAppender(name, a);
            }
            else if (type == "udp") {
                ap = BuildUdpAppender(name, a);
            }
            else if (type == "elastic")
            {
                ap = new ElasticSearchAppender(a);
            }
            else {
                selflog.Add(new LoggyMessage (LogLevel.Error,  "unknown appender type " + type ));
                return ap;
            }
            return ap;
        }

        private ILogAppender BuildUdpAppender(string name, XElement e) {
            var host = e.Attr("host", "127.0.0.1");
            var port = e.Attr("port", "7071").ToInt();
            var appender = new UdpAppender(host, port) {
                Format = e.Attr("format").Replace("%{","${"),
                Level = e.Attr("level","All").To<LogLevel>()
            };
            return appender;
        }

        private ILogAppender BuildConsoleAppender(string name, XElement e)
        {
            var appender = new ConsoleAppender();
            appender.Format = e.Attr("format").Replace("%{", "${");
            appender.Level = e.Attr("level","All").To<LogLevel>();
            return appender;
        }

        public ILoggy Get(string name = null, Action<ILoggy> setup= null) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = "default";
            }
            
            var result = loggers.GetOrAdd(name, n => {
                var l = new DefaultLoggy {Name = n};
                if (n != "default") {
                    ILoggy parent = null;
                    if (n.Contains(".")) {
                        var parts = n.Split('.');
                        var path = string.Join(".", parts.Take(parts.Length - 1));
                        parent = Get(path);
                    }
                    else {
                        parent = Get();
                    }


                    l.SubLoggers.Add(parent);
                    l.Level = parent.Level;
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

        public void SetContainerContext(IContainer container, IComponentDefinition component) {
          
        }

        public void OnContainerRelease() {
     
        }

        public void OnContainerCreateInstanceFinished() {
            if (null != ConfigProvider) {
                var e = ConfigProvider.GetConfig();
                if (null != e) {
                    InitializeFromXml(e);
                }
            }
        }
    }
}