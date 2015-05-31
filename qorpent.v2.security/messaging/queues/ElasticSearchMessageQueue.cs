using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using qorpent.v2.security.utils;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.messaging.queues {
    [ContainerComponent(Lifestyle.Singleton, "elastic.messagequeue", ServiceType = typeof (IMessageQueue))]
    public class ElasticSearchMessageQueue : InitializeAbleService, IMessageQueue {
        private const string getNotSendMessages =
            @"{""size"":${size},""query"":{""bool"":{""must"":[{""term"":{""WasSent"":{""value"":""false""}}},{""range"":{""StartTime"":{""lte"":""now""}}}]}}}";

        public ElasticSearchMessageQueue() {
            EsClient = new ElasticSearchClient();
            Index = "messaging";
            Type = "message";
        }

        public ElasticSearchClient EsClient { get; private set; }
        public bool Enabled { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }

        public PostMessage PushMessage(PostMessage message) {
            if (!Enabled) {
                throw new Exception("not available");
            }


            if (message.CreateTime.Year <= 1900) {
                message.CreateTime = DateTime.Now.ToUniversalTime();
            }
            if (message.StartTime.ToUniversalTime() < DateTime.Now.ToUniversalTime()) {
                message.StartTime = DateTime.Now.ToUniversalTime();
            }
            if (message.SentTime.Year <= 1900) {
                message.SentTime = DateTime.MinValue.ToUniversalTime();
            }
            if (string.IsNullOrWhiteSpace(message.Id)) {
                var json = message.stringify() + Guid.NewGuid();
                message.Id = "M" + json.GetMd5();
            }
            Store(message);
            return message;
        }

        public PostMessage GetMessage(string id) {
            var result = EsClient.ExecuteCommand("/" + Index + "/" + Type + "/" + id);
            if (null == result) {
                throw EsClient.LastError;
            }
            var j = result.jsonify();
            if (!j.bul("found")) {
                return null;
            }
            var pm = CreateFromJson(j);
            return pm;
        }

        public void MarkSent(string id) {
            EsClient.ExecuteCommand(BaseUrl() + "/" + id + "/_update?refresh=true", new {
                doc = new {
                    WasSent = true,
                    SentTime = DateTime.Now.ToUniversalTime()
                }
            }.stringify());
        }

        public IEnumerable<PostMessage> SearchMessages(object query) {
            var q = "";
            if (query is string) {
                q = (string) query;
                if (!q.StartsWith("{")) {
                    q = new {
                        query = new {
                            query_string = new {
                                query = q
                            }
                        }
                    }.stringify();
                }
            }
            else {
                q = query.stringify();
            }
            var result = EsClient.ExecuteCommand(BaseUrl() + "/_search", q);
            if (null == result) {
                throw EsClient.LastError;
            }
            var j = result.jsonify();
            var hits = j.arr("hits.hits");
            if (null == hits) {
                yield break;
            }
            foreach (var hit in hits) {
                yield return CreateFromJson(hit);
            }
        }

        public IEnumerable<PostMessage> GetRequireSendMessages(int count = -1) {
            if (count <= 0) {
                count = 10;
            }
            var q = getNotSendMessages.Replace("${size}", count.ToString());
            return SearchMessages(q);
        }

        public override void InitializeFromXml(XElement x) {
            var e = x.Element("messaging");
            if (null != e) {
                foreach (var element in e.Elements("queue")) {
                    if (element.Attr("code") == "elastic") {
                        Enabled = true;
                        var _ref = element.Attr("ref");
                        if (!string.IsNullOrWhiteSpace(_ref)) {
                            var _e = x.Element("elastic");
                            if (null == _e) {
                                throw new Exception("invalid ref");
                            }
                            _e = _e.Element(_ref);
                            if (null == _e) {
                                throw new Exception("invalid ref");
                            }
                            ReadSingle(_e);
                        }
                        ReadSingle(element);
                    }
                }
            }
        }

        private void ReadSingle(XElement e) {
            Index = e.Attr("index", Index);
            Type = e.Attr("type", Type);
            Enabled = e.Attr("active", "true").ToBool();
        }

        private static PostMessage CreateFromJson(object j) {
            var pm = new PostMessage();
            pm.Id = j.str("_id");
            var _src = j.map("_source");
            pm.Addresses = _src.arr("Addresses").Select(_ => _.ToStr()).ToArray();
            pm.From = _src.str("From");
            pm.CanUseDefault = _src.bul("CanUseDefault");
            pm.CreateTime = _src.date("CreateTime");
            pm.StartTime = _src.date("StartTime");
            pm.Message = _src.str("Message");
            pm.Subject = _src.str("Subject");
            pm.SentTime = _src.date("SentTime");
            pm.WasSent = _src.bul("WasSent");
            pm.Tags = _src.map("Tags");
            return pm;
        }

        private void Store(PostMessage message) {
            var result = EsClient.ExecuteCommand(BaseUrl() + "/" + message.Id + "?refresh=true", message.stringify());
            if (null == result) {
                throw EsClient.LastError;
            }
        }

        private string BaseUrl() {
            return "/" + Index + "/" + Type;
        }
    }
}