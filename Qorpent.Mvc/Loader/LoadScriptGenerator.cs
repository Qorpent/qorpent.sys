﻿using System;
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
            sb.Append("document.write(\"");
            sb.Append("<link rel='stylesheet' href='styles/");
            sb.Append(item.Value);
            sb.Append("' type='text/css' />");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GenerateScript(LoadItem item, StringBuilder sb) {
            sb.Append("document.write(\"");
            sb.Append("<script src='scripts/");
            sb.Append(item.Value);
            sb.Append("' type='text/javascript' defer='defer' ></script>");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GenerateMeta(LoadItem item, StringBuilder sb) {
            sb.Append("document.write(\"");
            sb.Append("<meta ");
            sb.Append(item.Value);
            sb.Append(" />");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GenerateLink(LoadItem item, StringBuilder sb) {
            sb.Append("document.write(\"");
            sb.Append("<link ");
            sb.Append(item.Value);
            sb.Append(" />");
            sb.Append("\");");
            sb.AppendLine();
        }

        private void GeneratePrePackage(LoadPackage pkg, StringBuilder sb) {
            sb.AppendLine("/* auto generated pkg "+pkg.Code+" ("+string.Join(",",pkg.Dependency)+") started */ ");
        }

        private void GeneratePreContent(LoadPackage[] set, StringBuilder sb) {
            sb.AppendLine("/* auto generated load set started */ ");
            sb.AppendLine("window.templates = window.templates || {};");

        }
    }
}