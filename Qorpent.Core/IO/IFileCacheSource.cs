using System;
using System.IO;

namespace Qorpent.IO {
    /// <summary>
    /// Интерфейс источника файлов для кэшевого резольвера файлов
    /// </summary>
    public interface IFileCacheSource {
        /// <summary>
        /// Корневая директория или URL
        /// </summary>
        string Root { get; set; }
        /// <summary>
        /// Признак "главного" источника
        /// </summary>
        bool IsMaster { get; set; }
        /// <summary>
        /// Возвращает NULL или источник потока при наличии запрошенного файла
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Func<Stream> GetStreamer(string name);
    }
}