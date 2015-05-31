using System;
using System.Collections.Generic;
using Qorpent.IO;
using Qorpent.IO.Net;

namespace qorpent.v2.security.utils {
    public class ElasticSearchClient {
        public ElasticSearchClient() {
            ConnectionTimeout = 200;
            Urls = new List<string> {"http://127.0.0.1:9200"};
        }

        public IList<string> Urls { get; set; }
        public int ConnectionTimeout { get; set; }
        public bool InvalidConnection { get; set; }
        public Exception LastError { get; set; }
        public DateTime LastPing { get; set; }

        public string ExecuteCommand(string cmd, string post = null, string method = "") {
            lock (this) {
                if (!cmd.StartsWith("/")) {
                    cmd = "/" + cmd;
                }
                string json;
                for (var i = 0; i < Urls.Count; i++) {
                    var url = Urls[i] + cmd;
                    try {
                        var cli = new HttpClient {ConnectionTimeout = ConnectionTimeout};
                        json = cli.GetString(url, post, _ => {
                            if (!string.IsNullOrWhiteSpace(method)) {
                                _.Method = method;
                            }
                        });
                        InvalidConnection = false;
                        if (i != 0) {
                            var best = Urls[i];
                            Urls.RemoveAt(i);
                            Urls.Insert(0, best);
                        }
                        return json;
                    }
                    catch (IOException e) {
                        if (i < Urls.Count - 1) {
                            continue;
                        }
                        InvalidConnection = true;
                        LastError = e;
                        return null;
                    }
                    finally {
                        LastPing = DateTime.Now;
                    }
                }
                return null;
            }
        }
    }
}