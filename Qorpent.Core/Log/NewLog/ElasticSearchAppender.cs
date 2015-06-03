using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using qorpent.v2.security.utils;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log.NewLog {
    public class ElasticSearchAppender : AppenderBase {

        public ElasticSearchAppender() {
            Client = new ElasticSearchClient();
            Index = "qptlogs";
            Type = "m";
        }

        public ElasticSearchAppender(XElement e) : this() {
            Initialize(e);
        }

        private void Initialize(XElement e) {
            var _ref = e.Attr("ref");
            if (null != _ref) {
                var root = e.toRoot();
                var elastic = root.Element("elastic");
                if(null==elastic)throw new Exception("invalid ref");
                var cfg = elastic.Element(_ref);
                if(null==cfg) throw new Exception("invalid ref");
                InitializeDirect(cfg);
            }
            InitializeDirect(e);
        }

        private void InitializeDirect(XElement e) {
            Client.Urls = e.Attr("urls", "127.0.0.1:9200").SmartSplit(false, true, ',',';');
            Index = e.Attr("index", Index);
            Type = e.Attr("estype", Type);
            Level = e.Attr("level", Level.ToStr()).To<LogLevel>();
        }

        public string Index { get; set; }
        public string Type { get; set; }

        public ElasticSearchClient Client { get; set; }
        public override void Write(LoggyMessage message) {
            if(message.Level<Level)return;
            object error = null;
            if (null != message.Exception) {
                error = new {
                    type = message.Exception.GetType().Name,
                    message = message.Exception.Message,
                    stack = message.Exception.StackTrace
                };
            }
            
            var id = GetId(message);
            var m = new {
                level = message.Level,
                timestamp = message.Timestamp.ToUniversalTime(),
                logger = message.LoggerName,
                message = message.Message,
                error,
                user = message.UserName,
                host = Environment.MachineName,
                _id = id
            };
            var j = m.stringify();
            Client.ExecuteCommand("/" + Index + "/" + Type + "/" + id, j);
        }

        private object GetId(LoggyMessage message) {
            return message.Timestamp.ToUniversalTime().ToString("yyyyMMMMddHHmmss_") +
                   Guid.NewGuid().ToString().GetMd5().Substring(0, 6);
        }
    }
}