using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Класс считывания элементов загрузки из XML
    /// </summary>
    public class LoadPackageReader : ILoadPackageReader {
        /// <summary>
        /// Десериализует сырой набор пакетов из XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public IEnumerable<LoadPackage> Read(XElement xml) {
            foreach (var pkge in xml.Elements("pkg")) {
                yield return ReadPackage(pkge);
            }
        }

        private LoadPackage ReadPackage(XElement pkge) {
            var result = new LoadPackage {Code = pkge.Attr("code")};
            var level = pkge.Attr("level");
            if (!string.IsNullOrWhiteSpace(level)) {
                result.Level = (LoadLevel) Enum.Parse(typeof (LoadLevel), level, true);
            }
            var deps = pkge.Elements("uses");
            foreach (var dep in deps) {
                result.Dependency.Add(dep.Attr("code"));
            }
            foreach (var item in pkge.Elements("load")) {
                result.Items.Add(ReadItem(item));
            }
            foreach (var item in pkge.Elements("widget")) {
                ReadWidget(result, item);
            }

            return result;
        }

        private void ReadWidget(LoadPackage result, XElement item) {
            var level = LoadLevel.Guest;
            var levele = item.Attr("level");
            if (!string.IsNullOrWhiteSpace(levele))
            {
                level = (LoadLevel)Enum.Parse(typeof(LoadLevel), levele, true);
            }
            var code = item.Attr("code");
            result.Items.Add(new LoadItem {Level = level, Type = LoadItemType.Script, Value = code + ".js"});
            result.Items.Add(new LoadItem { Level = level, Type = LoadItemType.Style, Value = code + ".css" });
            result.Items.Add(new LoadItem { Level = level, Type = LoadItemType.Template, Value = code + ".tpl.html" });
        }

        private LoadItem ReadItem(XElement ie) {
            var result = new LoadItem {Value = ie.Attr("code")};
            var level = ie.Attr("level");
            if (!string.IsNullOrWhiteSpace(level))
            {
                result.Level = (LoadLevel)Enum.Parse(typeof(LoadLevel), level, true);
            }
            if (result.Value.EndsWith(".js")) {
                result.Type = LoadItemType.Script;
            }else if (result.Value.EndsWith(".css")) {
                result.Type = LoadItemType.Style;
            }else if (result.Value.Contains("rel=")) {
                result.Type = LoadItemType.Link;
            }else if (result.Value.EndsWith(".html")) {
                result.Type = LoadItemType.Template;
            }
            else {
                result.Type = LoadItemType.Meta;
            }
            return result;
        }
    }
}