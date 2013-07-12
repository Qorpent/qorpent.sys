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
        }

        private void GeneratePkg(LoadPackage pkg, StringBuilder sb) {
            GeneratePrePackage(pkg, sb);
            foreach (var item in pkg.Items) {
                GenerateItem(item, sb);
            }
            GeneratePostPackage(pkg, sb);
        }

        private void GeneratePostPackage(LoadPackage pkg, StringBuilder sb) {
            sb.AppendLine("/* auto generated pkg " + pkg.Code + " finished */ ");
        }

        private void GenerateItem(LoadItem item, StringBuilder sb) {
            switch (item.Type) {
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
                default:
                    throw new Exception("unknown type");
            }
        }

        private void GenerateStyle(LoadItem item, StringBuilder sb) {
            sb.Append("document.write(\"");
            sb.Append("<link rel='stylesheet' href='styles/");
            sb.Append(item.Value);
            sb.Append("' type='text/css' ></link>");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GenerateScript(LoadItem item, StringBuilder sb) {
            sb.Append("document.write(\"");
            sb.Append("<script src='scripts/");
            sb.Append(item.Value);
            sb.Append("' type='text/javascript' ></script>");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GenerateMeta(LoadItem item, StringBuilder sb) {
            
        }

        private void GenerateLink(LoadItem item, StringBuilder sb) {
            
        }

        private void GeneratePrePackage(LoadPackage pkg, StringBuilder sb) {
            sb.AppendLine("/* auto generated pkg "+pkg.Code+" started */ ");
        }

        private void GeneratePreContent(LoadPackage[] set, StringBuilder sb) {
            sb.AppendLine("/* auto generated load set started */ ");
        }
    }
}