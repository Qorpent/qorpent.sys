using System;
using System.Collections.Generic;
using Qorpent.Experiments;

namespace bit.cross.accident.services.query {
    public static class EsQueryUtils {
        public static IDictionary<string, object> QSetSortDesc(this IDictionary<string, object> query, string fieldname) {
            query["sort"] = new Dictionary<string,object> {[fieldname]="desc"};
            return query;
        }
        public static IDictionary<string, object> QSetSort(this IDictionary<string, object> query, string fieldname)
        {
            query["sort"] = new Dictionary<string, object> {[fieldname] = "asc" };
            return query;
        }
        public static IDictionary<string, object> QRemove(this IDictionary<string, object> query, string fieldname) {
            query.Remove(fieldname);
            return query;
        }
        public static IDictionary<string, object> QSetSize(this IDictionary<string, object> query, int size) {
            query["size"] = size;
            return query;
        }
        public static IDictionary<string, object> QSetFrom(this IDictionary<string, object> query, int size)
        {
            query["from"] = size;
            return query;
        }

        public static IEnumerable<string> QExtractQueryStrings(this IDictionary<string, object> query) {
            foreach (var n in query.collect((_,s) => {
                var m = _ as IDictionary<string, object>;
                if (null != m) {
                    if (m.ContainsKey("query_string")) {
                        return NodeFilter.Return | NodeFilter.Stop;
                    }
                }
                return NodeFilter.None;

            })) {
                yield return n.Value.str("query_string.query").Trim();
            }
        }
        
        public static IDictionary<string,object> QEnsureMust(this IDictionary<string, object> query, object condition) {
            var qs = condition as string;
            if (null != qs) {
                condition = new {
                    query_string = new {
                        query = qs
                    }
                };
            }
            var q = query.map("query");
            if (null == q) {
                query["query"] = new Dictionary<string, object> {{"bool", new {must = new {}}}}.jsonify();
                q = query.map("query");
            }
            var bl = q.map("bool");
            if (null == bl) {
                bl = new Dictionary<string, object>();
                var mst = new List<object>();
                foreach (var o in q) {
                    mst.Add(new Dictionary<string, object> {{o.Key, o.Value}});

                    ;
                }
                q.Clear();
                bl["must"] = mst.ToArray();
                q["bool"] = bl;
            }
            var mustlist = new List<object>();
            var must = bl.arr("must");
            if (null != must) {
                foreach (var o in must) {
                    mustlist.Add(o);
                }
            }
            mustlist.Add(condition.jsonify());
            bl["must"] = mustlist.ToArray();
            return query;
        }
    }
}