using System;
using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     Представление файла
    /// </summary>
    public class FileEntity : IFileEntity {
        /// <summary>
        ///     Имя файла
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        ///     полный путь до файла
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        ///     Дата публикации файла в хранилище
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        ///     Версия файла
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        ///     Возвращает путь до директории, в которой 
        ///     находится файл
        /// </summary>
        /// <returns></returns>
        public string GetDirectoryName() {
            return System.IO.Path.GetDirectoryName(Path);
        }
        /// <summary>
        ///     Возвращает представление директории, в которой находится файл
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo GetDirectory() {
            return new DirectoryInfo(GetDirectoryName());
        }
    }
}
