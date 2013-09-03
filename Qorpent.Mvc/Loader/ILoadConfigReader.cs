using System.Xml.Linq;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Интерфейс загрузчика конфигурации приложения
    /// </summary>
    public interface ILoadConfigReader {
        /// <summary>
        /// Загружест BXL конфигурацию приложения
        /// </summary>
        /// <returns></returns>
        XElement LoadConfig();
    }
}