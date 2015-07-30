using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.Experiments
{
    public static class JsonExtend
    {
        /// <summary>
        /// Выплоняет простую логику расширения
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Extend(object target, object source) {
            if (target is IDictionary<string, object> && source is IDictionary<string, object>) {
                return ExtendDictionaries((IDictionary<string, object>) target, (IDictionary<string, object>) source);
            }
            if (target is object[] && source is object[]) {
                return ExtendArrays((object[]) target, (object[]) source);
            }

    
            return source;
        }

        public static object ExtendNotJsoned(object target, object source) {
            if (null == source) return Json.Jsonify(target);
            return Extend(Json.Jsonify(target), Json.Jsonify(source));
        }

        private static object ExtendArrays(object[] target, object[] source) {
            var trg = new List<object>(target);
            foreach (var o in source) {
                if (o is IDictionary<string, object>) {
                    var dict = o as IDictionary<string, object>;
                    if (dict.ContainsKey("_remove")) {
                        var remover = dict["_remove"];
                        DoRemove(trg, remover);
                    }
                    else if (dict.ContainsKey("_id")) {
                        var ex = FindById(trg, dict["_id"]);
                        if (null != ex) {
                            trg.Remove(ex);
                            trg.Add(Extend(ex, dict));
                        }
                        else {
                            trg.Add(dict);    
                        }
                        
                    }
                    else {
                        trg.Add(dict);
                    }

                }
                else  {
                    if (!trg.Contains(o)) {
                        trg.Add(o);
                    }
                }
            }
            return trg.ToArray();
        }

        private static void RemoveById(List<object> trg, object id) {
            var ex = FindById(trg, id);
            if (null != ex) {
                trg.Remove(ex);
            }
        }

        private static IDictionary<string, object> FindById(List<object> trg, object id) {
            var ex =
                trg.OfType<IDictionary<string, object>>()
                    .FirstOrDefault(_ => Equals(_.SafeGet("_id"), id));
            return ex;
        }

        private static void DoRemove(List<object> trg, object remover) {
            if (remover is IDictionary<string, object>) {
                var dict = remover as IDictionary<string, object>;
                if (dict.ContainsKey("_id")) {
                    RemoveById(trg,dict["_id"]);
                }
                if (dict.ContainsKey("_val")) {
                    DoRemove(trg,dict["_val"]);
                }
                if (dict.ContainsKey("_idx")) {
                    trg.RemoveAt(dict["_idx"].ToInt());
                }
            }
            else if (remover is object[]) {
                var a = remover as object[];
                foreach (var o in a) {
                    DoRemove(trg, o);
                }
            }
            else {
                
                    trg.Remove(remover);
                
            }
        }

        private static object ExtendDictionaries(IDictionary<string, object> target, IDictionary<string, object> source) {
            foreach (var o in source) {
                if (Equals("_remove_", o.Value)) {
                    target.Remove(o.Key);
                    continue;
                }
                if (!target.ContainsKey(o.Key)) {
                    target[o.Key] = o.Value;
                    continue;
                }
                target[o.Key] = Extend(target[o.Key], o.Value);
            }
            return target;
        }
    }
}
