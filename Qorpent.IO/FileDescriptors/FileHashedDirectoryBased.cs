using Qorpent.IO.DirtyVersion.Storage;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на HashedDirectory
    /// </summary>
    public class FileHashedDirectoryBased : IFile {
        /// <summary>
        ///     Экземпляр класса нативного хранилища
        /// </summary>
        private readonly HashedDirectory _hashedDirectoryStorage;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileDescriptor Descriptor { get; private set; }
        /// <summary>
        ///     Дескриптор файла, основанный на HashedDirectory
        /// </summary>
        /// <param name="hashedDirectory">Экземпляр настроенного движка хранилища</param>
        /// <param name="fileDescriptor">Представление файла</param>
        public FileHashedDirectoryBased(HashedDirectory hashedDirectory, IFileDescriptor fileDescriptor) {
            _hashedDirectoryStorage = hashedDirectory;
            Descriptor = fileDescriptor;
        }
        /// <summary>
        ///     Получение потока до файла
        /// </summary>
        /// <param name="access">Уровень доступа до потока</param>
        /// <returns>Поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _hashedDirectoryStorage.Open(Descriptor.Path);
        }
    }
}
