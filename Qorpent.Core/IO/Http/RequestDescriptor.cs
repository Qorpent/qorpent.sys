using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Http {
    public class RequestDescriptor {
        public RequestDescriptor() {
            Headers = new Dictionary<string, string>();
        }
        public Uri Uri { get; set; }
        public string PostData { get; set; }
        public string Method { get; set; }
        public IDictionary<string, string> Headers { get; private set; }

      
        public static RequestDescriptor Create(string url, object options = null)
        {
            var j = (options ?? new { }).jsonifymap();
            j["url"] = url;
            return Create(j);
        }
        public static RequestDescriptor Create(object options = null)
        {
            var j = (options ?? new { }).jsonifymap();
            var method = (j.str("method") ?? "DEFAULT").ToUpperInvariant();
            var url = j.str("url") ?? "http://127.0.0.1/";
            var param = j.map("param");
            var data = j.get("data");
            var headers = j.map("headers");
            if (method == "DEFAULT") {
                if (null != data) {
                    method = "POST";
                }
                else {
                    method = "GET";
                }
            }
            if (method == "GET")
            {
                if (null != data)
                {
                    throw new Exception("cannot use post data in get queries");
                }
            }
            var enc = j.str("enc") ?? "urlencode";
            
            var querydata = enc == "json" ? param.stringify() : EscapeQueryData(param);
            if (querydata.StartsWith("{") && method == "GET") {
                querydata = System.Uri.EscapeDataString(querydata);
            }
            var uri = new Uri(url);
            var query = uri.Query;
            if ((method == "GET" || null != data) && !string.IsNullOrWhiteSpace(querydata) ) {
                if (string.IsNullOrWhiteSpace(query)) {
                    query = "?" + querydata;
                }
                else {
                    query = query + "&" + querydata;
                }
            }
            var newurl = String.Format("{0}{1}{2}{3}", uri.Scheme,Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
            if (!string.IsNullOrWhiteSpace(query)) {
                newurl += query;
            }
            uri = new Uri(newurl);
            var postdata = "";
            if (method == "POST") {
                if (null == data) {
                    postdata = querydata ?? "";
                }
                else {
                    postdata = data.stringify();
                }
            }


            var result = new RequestDescriptor {Uri = uri, Method = method, PostData = postdata};

            if (null != headers) {
                foreach (var header in headers) {
                    result.Headers[header.Key] = header.Value.ToStr();
                }
            }

            return result;
        }

        private static string EscapeQueryData(IDictionary<string, object> dictionary) {
            if (null == dictionary) return "";
            return string.Join("&",
                dictionary.Select(_ => Uri.EscapeDataString(_.Key) + "=" + 
                    Uri.EscapeDataString(GetValueString(_.Value)).Replace(" ","%20")));
        }

        private static string GetValueString(object val) {
            if (val is string) {
                var s = val as string;
                if(s[0]!='"' && s[0]!='[' && s[0]!='{')
                {
                    return s;
                }
            }
            var jval = val.jsonify();
            if (jval is string) return jval as string;
            if (jval.GetType().IsValueType) return jval.ToStr();
            return jval.stringify();
        }
    }
}