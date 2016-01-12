using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Qorpent.BSharp;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;
using Map = System.Collections.Generic.Dictionary<string,object>;
using IMap = System.Collections.Generic.IDictionary<string,object>;
namespace bit.cross.accident.services.query {
    public static class EsQueryUtils {
        public static IDictionary<string, object> QSetSortDesc(this IDictionary<string, object> query, string fieldname) {
            query["sort"] = new Dictionary<string,object> {[fieldname]="desc"};
            return query;
        }

        public static IMap QEnsureFields(this IMap query, params string[] fields) {
            if (null == fields || 0 == fields.Length)
            {
                return query;
            }

            if (!query.ContainsKey("fields")) {
                query["fields"] = fields;
            }
            else {
                var l = query.arr("fields").ToList();
                foreach (var field in fields) {
                    if (!l.Contains(field)) {
                        l.Add(field);

                    }
                }
            }
            return query;
            ;
        }
        public static IDictionary<string, object> QIncludeSource(this IDictionary<string, object> query,params string[] fields) {
            if (null == fields || 0 == fields.Length) {
                return query;
            }
            if (!query.ContainsKey("_source")) {
                query["_source"] = true;
            }
            query.QNormalizeSource();
            if (Equals(false, query["_source"])) {
                return query;
            }
            var src = query.map("_source");
            if (!src.ContainsKey("include")) {
                src["include"] = new object[] {fields};
            }
            else {
                var l = src.arr("include").ToList();
                l.Remove("*");
                if (!l.Contains(fields)) {
                    l.Add(fields);
                }
                src["include"] = l.ToArray();
            }
            return query;
        }
        public static IDictionary<string, object> QExcludeSource(this IDictionary<string, object> query, string fields)
        {
            if (null == fields || 0 == fields.Length)
            {
                return query;
            }
            if (!query.ContainsKey("_source"))
            {
                query["_source"] = true;
            }
            query.QNormalizeSource();
            if (Equals(false, query["_source"]))
            {
                return query;
            }
            var src = query.map("_source");
            if (!src.ContainsKey("exclude"))
            {
                src["exclude"] = new object[] { fields };
            }
            else
            {
                var l = src.arr("exclude").ToList();
                l.Remove("*");
                if (!l.Contains(fields))
                {
                    l.Add(fields);
                }
                src["exclude"] = l.ToArray();
            }
            return query;
        }
        public static IDictionary<string, object> QNormalizeSource(this IDictionary<string, object> query) {
            if (query.ContainsKey("_source"))
            {
                var _src = query["_source"];
                if ((_src is bool) && _src.ToBool())
                {
                    _src = new Map
                    {
                        ["include"] = new object[] { "*" }
                    };

                }
                else if (_src is Array)
                {
                    _src = new Map
                    {
                        ["include"] = _src
                    };

                }
                query["_source"] = _src;
            }
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

        public static IDictionary<string, object> QSetHighlight(this IDictionary<string, object> query, string field,
            int number_of_fragments = 1, int fragment_size =100000) {
            if (number_of_fragments == 0) {
                number_of_fragments = 1;
            }
            if (fragment_size == 0) {
                fragment_size = 100000;
            }
            var h = query.map("highlight") ?? ((Map) (query["highlight"] = new Map()));
            if (!h.ContainsKey("fields")) {
                h["fields"] = new Map();
            }
            var fields = h.map("fields");
            if (!fields.ContainsKey(field)) {
                fields[field] = new Map();
            }
            var fld = fields.map(field);
            fld[nameof(number_of_fragments)] = number_of_fragments;
            fld[nameof(fragment_size)] = fragment_size;
            return query;
        }


        public static IDictionary<string, object> GetIdsCondition(params string[] ids) {
            ids = (ids ?? new string[] {}).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            if(ids.Length==0)throw new Exception("no ids for condition");
            return new Map {
                ["ids"] = new Map {
                    ["values"] = ids.OfType<object>().ToArray()
                }
            };
        } 

        


        
        public static IDictionary<string,object> QEnsureBoolPart(this IDictionary<string, object> query, object condition,string boolpart = "must") {
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
                query["query"] = new Dictionary<string, object> {{"bool", new Dictionary<string,object>{
                    [boolpart] = new {}
                }}}.jsonify();
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
            var must = bl.arr(boolpart);
            if (null != must) {
                foreach (var o in must) {
                    mustlist.Add(o);
                }
            }
            mustlist.Add(condition.jsonify());
            bl[boolpart] = mustlist.ToArray();
            return query;
        }

        public static IDictionary<string, object> QNoSource(this IDictionary<string, object> q) {
            q["_source"] = false;
            return q;
        }

        public static IDictionary<string, object> QSetFieldAgg(this IDictionary<string,object> query, string field, string agg, bool onlyaggs = false)
        {
            query["aggs"] = new Dictionary<string, object>
            {
                [agg] = new Dictionary<string, object>
                {
                    [agg] = new Dictionary<string, object>
                    {
                        ["field"] = field
                    }
                }
            };
            if (onlyaggs)
            {
                query.QSetSize(0);
                query.QNoSource();
            }
            return query;
        }

        public static IDictionary<string,object> GetFieldAgg(string field, string agg, bool onlyaggs = false) {
            var result = new Dictionary<string,object>();
            result["aggs"] = new Dictionary<string,object> {
                [agg] = new Dictionary<string, object> {
                    [agg] = new Dictionary<string, object> {
                        ["field"] = field
                    }
                }
            };
            if (!onlyaggs) {
                result.QSetSize(0);
                result.QNoSource();
            }
            return result;
        }
    }
}