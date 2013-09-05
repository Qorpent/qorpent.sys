using System;

namespace Qorpent.IO {
    /// <summary>
    ///     Внутреннее представление файла
    /// </summary>
    public interface IFileEntity {
        /// <summary>
        ///     полный путь до файла
        /// </summary>
        string Path { get; set; }
        /// <summary>
        ///     Дата публикации файла в хранилище
        /// </summary>
        DateTime DateTime { get; set; }
        /// <summary>
        ///     Версия файла
        /// </summary>
        string Version { get; set; }
    }
}