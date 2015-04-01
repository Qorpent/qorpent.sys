using System;
using System.Xml.Linq;

namespace Qorpent.Utils.IO {
    /// <summary>
    /// Интерфейс для описания версионированного ресурса
    /// </summary>
    public interface IVersionedDescriptor {
        /// <summary>
        /// Хэщ версии
        /// </summary>
        string Hash { get; set; }

        /// <summary>
        /// Время формирования версии
        /// </summary>
        DateTime Version { get; set; }

        /// <summary>
        /// Логическое или локальное имя
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Полное имя файла
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Расширенные атрибуты текстового файла
        /// </summary>
        XElement Header { get; set; }
    }
}