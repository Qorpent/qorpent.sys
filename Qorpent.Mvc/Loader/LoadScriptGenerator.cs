using System;
using System.Text;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Генератор JS скрипта с загрузчиком
    /// </summary>
    public class LoadScriptGenerator : ILoadScriptGenerator {
        /// <summary>
        /// Сформировать скрипт из переданного нормализованного набора пакетов
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public string Generate(LoadPackage[] set) {
            var sb = new StringBuilder();
            GeneratePreContent(set, sb);
            foreach (var pkg in set) {
                GeneratePkg(pkg, sb);
            }
            GeneratePostContent(set, sb);
            return sb.ToString();
        }

        private void GeneratePostContent(LoadPackage[] set, StringBuilder sb) {
            sb.AppendLine("/* auto generated load set finished */ ");
            sb.AppendLine("})(window, window.qweb.embedStorage._sys__myactions)");
        }

        private void GeneratePkg(LoadPackage pkg, StringBuilder sb) {
            GeneratePrePackage(pkg, sb);
            foreach (var item in pkg.Items) {
                GenerateItem(item, sb);
            }
            GeneratePostPackage(pkg, sb);
        }

        private void GeneratePostPackage(LoadPackage pkg, StringBuilder sb) {
            sb.AppendLine("}");
            sb.AppendLine("/* auto generated pkg " + pkg.Code+ " finished */ ");
        }

        private void GenerateItem(LoadItem item, StringBuilder sb) {
            sb.AppendFormat("if(allowed('','{0}')){{", item.Command);
            var type = item.Type;
            if (type == LoadItemType.Unknown) {
                type = DetermineType(item);
            }
            switch (type) {
                    case LoadItemType.Link:
                    GenerateLink(item, sb);
                    break;
                    case LoadItemType.Meta:
                    GenerateMeta(item, sb);
                    break;
                    case LoadItemType.Script:
                    GenerateScript(item, sb);
                    break;
                    case LoadItemType.Style:
                    GenerateStyle(item, sb);
                    break;
                    case LoadItemType.Template:
                    GenerateTemplate(item, sb);
                    break;
                default:
                    throw new Exception("unknown type");
            }
            sb.AppendLine("}");
        }

        private LoadItemType DetermineType(LoadItem item) {
            if (item.Value.EndsWith(".js")) return LoadItemType.Script;
            if (item.Value.EndsWith(".css"))return LoadItemType.Style;
            if (item.Value.EndsWith(".html"))return LoadItemType.Template;
            if (item.Value.Contains("rel=")) return LoadItemType.Link;
            return LoadItemType.Meta;
          }

        private void GenerateTemplate(LoadItem item, StringBuilder sb) {
            sb.Append("$.ajax({ url: 'tpl/" + item.Value + "', async: false })");
            sb.Append(".success(function(data){templates['"+item.Value.Replace(".html","").Replace(".tpl","") +"'] = data;});");
            sb.AppendLine();
        }

        private void GenerateStyle(LoadItem item, StringBuilder sb) {
			sb.AppendFormat("$(document.head).append($('<link/>').attr({{rel:'stylesheet', href:'styles/{0}'}}));\r\n", item.Value);

        }

        private void GenerateScript(LoadItem item, StringBuilder sb) {
			sb.AppendFormat("$(document.head).append($('<script src=\"scripts/{0}\" async=\"false\" />'));\r\n", item.Value);
        }

        private void GenerateMeta(LoadItem item, StringBuilder sb) {
			sb.AppendFormat("$(document.head).append($('<meta {0} />'));\r\n", item.Value);
        }

        private void GenerateLink(LoadItem item, StringBuilder sb) {
			sb.AppendFormat("$(document.head).append($('<link {0} />'));\r\n", item.Value);
     
        }

        private void GeneratePrePackage(LoadPackage pkg, StringBuilder sb) {
            sb.AppendLine("/* auto generated pkg "+pkg.Code+" ("+string.Join(",",pkg.Dependency)+"):"+pkg.Arm+":"+pkg.Command+" started */ ");
            sb.AppendFormat("if(allowed('{0}','{1}')){{\r\n", pkg.Arm, pkg.Command);
        }

        private void GeneratePreContent(LoadPackage[] set, StringBuilder sb) {
            sb.AppendLine("/* auto generated load set started */ ");
            sb.AppendLine("(function(root,actions){");
            sb.AppendLine("root.templates = root.templates || {};");
            sb.AppendLine("root.arms = ((document.head.getElementsByClassName('qorpent-loader')[0].getAttribute('arm'))||'').split(',');");
            sb.AppendLine(@"function allowed(arm,command){
    if(!(arm||command))return true; //empty condition
    if(arm=='default'&&!command)return true; // default arm always exists
    if(arm!='default'&& $.inArray(arm, root.arms)==-1)return false; //arm not match
    if(!command) return true; //arm match, command empty
    var cmd = command.split(',');
    if(!!actions[cmd[0]]){
        if(!!actions[cmd[0]][cmd[1]]){
            return true; //command match
        }
    }
    return false; //arm or command not match
}
");

        }
    }
}