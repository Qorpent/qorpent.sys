using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     Дескриптор файла
    /// </summary>
    public interface IGeneralFileDescriptor {
        /// <summary>
        ///     Представление файла
        /// </summary>
        IFileEntity FileEntity { get; }
        /// <summary>
        ///     получить поток до файла
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        Stream GetStream(FileAccess access);
    }
}