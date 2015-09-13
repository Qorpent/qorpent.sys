using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Qorpent.BSharp;
using Qorpent.Experiments;
using Map = System.Collections.Generic.Dictionary<string,object>;
namespace bit.cross.accident.services.query {

    public enum UpdateOperation {
        Insert,
        Update,
        Ensure,
        Remove
    }
    public static class EsGroovyUtils {
        public static string BuildUpdateArrayScript(string arrayname, object item,
            UpdateOperation op = UpdateOperation.Update, string keyname = "key") {
            if (null == item) {
                throw new ArgumentNullException(nameof(item));
            }
            var sb = new StringBuilder();
            string n = "";
            sb.AppendLine("def _src = ctx._source;");
            sb.AppendLine("def _t = _src;");
            var path = arrayname.Split('.');
            for (var i = 0; i < path.Length - 1; i++) {
                n = path[i];
                if (op == UpdateOperation.Remove) {
                    sb.AppendLine(string.Format("if(!_t['{0}'])return;_t=_t['{0}'];", n));
                }
                else {
                    sb.AppendLine(string.Format("if(!_t['{0}']){{_t['{0}']=[,]}};_t=_t['{0}'];", n));
                }
            }
            n = path[path.Length - 1];
            if (op == UpdateOperation.Remove) {
                sb.AppendLine(string.Format("if(!_t['{0}'])return;_t=_t['{0}'];", n));
            }
            else {
                sb.AppendLine(string.Format("if(!_t['{0}']){{_t['{0}']=[]}};_t=_t['{0}'];", n));
            }
            var jval = item.stringify();
            if (item is string || item.GetType().IsValueType) {

                sb.AppendLine("_t.remove(" + jval + ");");
                if (op != UpdateOperation.Remove) {
                    sb.AppendLine("_t.add(" + jval + ");");
                }
            }
            else {
                item = item.jsonify();
                var grovyobj = jval.Replace("{", "[").Replace("}", "]") ;
                sb.AppendLine("def _ex = _t.find{it." + keyname + "==" + item.get(keyname).stringify() + "};");
                if (op == UpdateOperation.Remove || op == UpdateOperation.Insert) {
                    sb.AppendLine("if(_ex)_t.remove(_ex);");
                }
                if (op == UpdateOperation.Ensure || op==UpdateOperation.Update) {
                    sb.AppendLine("if(!_ex){_t.add(" + grovyobj + ")};");
                }
                if (op == UpdateOperation.Insert) {
                    sb.AppendLine("_t.add(" + grovyobj + ");");
                }
                if (op == UpdateOperation.Update) {
                    sb.AppendLine("if(!!_ex){");
                    foreach (var p in item.jsonifymap()) {
                        sb.AppendLine("_ex['" + p.Key + "']=" + p.Value.stringify() + ";");
                    }
                    sb.AppendLine("}");
                }
            }
            return sb.ToString();
        }
    }

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
    }
}