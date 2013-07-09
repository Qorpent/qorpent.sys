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
            var deps = pkge.ChooseAttr("depend", "dependency");
            if (!string.IsNullOrWhiteSpace(deps)) {
                foreach (var d in deps.SmartSplit()) {
                    result.Dependency[d] = null;
                }
            }
            foreach (var item in pkge.Elements("load")) {
                result.Items.Add(ReadItem(item));
            }

            return result;
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
            }
            else {
                result.Type = LoadItemType.Meta;
            }
            return result;
        }
    }
}