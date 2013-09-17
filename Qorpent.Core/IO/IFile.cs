using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     Дескриптор файла
    /// </summary>
    public interface IFile {
        /// <summary>
        ///     Представление файла
        /// </summary>
        IFileDescriptor Descriptor { get; }
        /// <summary>
        ///     получить поток до файла
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        Stream GetStream(FileAccess access);
    }
}