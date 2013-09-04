using System.Xml.Linq;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Статичный класс с экстеншенами для мапера
    /// </summary>
    static class VcsStorageMapperExtensions {
        /// <summary>
        ///     Проверяет, не является ли элемент уже удалённым
        /// </summary>
        /// <param name="element">XML представление элемента</param>
        public static bool IsRemovedElement(this XElement element) {
            var removed = element.Attribute("Removed");

            if (removed != null) {
                if (removed.Value == "true") {
                    return true;
                }
            }

            return false;
        }
    }
}
