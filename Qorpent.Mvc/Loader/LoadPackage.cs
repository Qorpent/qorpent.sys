using System.Collections.Generic;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Пакет загружаемого контента
    /// </summary>
    public class LoadPackage {
        /// <summary>
        /// def ctor
        /// </summary>
        public LoadPackage() {
            Items = new List<LoadItem>();
            Dependency = new Dictionary<string, LoadPackage>();
        }
        /// <summary>
        /// Код пакета
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Level of package
        /// </summary>
        public LoadLevel Level { get; set; }
        /// <summary>
        /// Элементы пакета
        /// </summary>
        public IList<LoadItem> Items { get; private set; }
        /// <summary>
        /// Зависимость от других пакетов
        /// </summary>
        public IDictionary<string,LoadPackage> Dependency { get; private set; }

        /// <summary>
        /// Создает копию пакета
        /// </summary>
        /// <returns></returns>
        public LoadPackage Clone() {
            var result = (LoadPackage)MemberwiseClone();
            result.Items = new List<LoadItem>();
            result.Dependency = new Dictionary<string, LoadPackage>();
            foreach (var ld in this.Dependency) {
                result.Dependency[ld.Key] = null;
            }
            foreach (var item in Items) {
                result.Items.Add(item);
            }
            return result;
        }
    }
}