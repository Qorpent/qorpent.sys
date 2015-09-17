using System;
using System.Text;
using Qorpent.Experiments;

namespace bit.cross.accident.services.query {
    public static class EsGroovyUtils {
        public static string BuildUpdateArrayScript(string arrayname, object item,
            ArrayUpdateOperation op = ArrayUpdateOperation.Update, string keyname = "key") {
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
                if (op == ArrayUpdateOperation.Remove) {
                    sb.AppendLine(string.Format("if(!_t['{0}'])return;_t=_t['{0}'];", n));
                }
                else {
                    sb.AppendLine(string.Format("if(!_t['{0}']){{_t['{0}']=[,]}};_t=_t['{0}'];", n));
                }
            }
            n = path[path.Length - 1];
            if (op == ArrayUpdateOperation.Remove) {
                sb.AppendLine(string.Format("if(!_t['{0}'])return;_t=_t['{0}'];", n));
            }
            else {
                sb.AppendLine(string.Format("if(!_t['{0}']){{_t['{0}']=[]}};_t=_t['{0}'];", n));
            }
            var jval = item.stringify();
            if (item is string || item.GetType().IsValueType) {

                sb.AppendLine("_t.remove(" + jval + ");");
                if (op != ArrayUpdateOperation.Remove) {
                    sb.AppendLine("_t.add(" + jval + ");");
                }
            }
            else {
                item = item.jsonify();
                var grovyobj = jval.Replace("{", "[").Replace("}", "]") ;
                sb.AppendLine("def _ex = _t.find{it." + keyname + "==" + item.get(keyname).stringify() + "};");
                if (op == ArrayUpdateOperation.Remove || op == ArrayUpdateOperation.Insert) {
                    sb.AppendLine("if(_ex)_t.remove(_ex);");
                }
                if (op == ArrayUpdateOperation.Ensure || op==ArrayUpdateOperation.Update) {
                    sb.AppendLine("if(!_ex){_t.add(" + grovyobj + ")};");
                }
                if (op == ArrayUpdateOperation.Insert) {
                    sb.AppendLine("_t.add(" + grovyobj + ");");
                }
                if (op == ArrayUpdateOperation.Update) {
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
}