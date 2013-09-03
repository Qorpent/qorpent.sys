using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Интерфейс сериализации пакета из XML
    /// </summary>
    public interface ILoadPackageReader {
        /// <summary>
        /// Десериализует сырой набор пакетов из XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        IEnumerable<LoadPackage> Read(XElement xml);
    }
}