using Qorpent.IO.DirtyVersion.Storage;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на HashedDirectory
    /// </summary>
    public class FileDescriptorHashedDirectoryBased : IGeneralFileDescriptor {
        /// <summary>
        ///     Экземпляр класса нативного хранилища
        /// </summary>
        private readonly HashedDirectory _hashedDirectoryStorage;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity FileEntity { get; private set; }
        /// <summary>
        ///     Дескриптор файла, основанный на HashedDirectory
        /// </summary>
        /// <param name="hashedDirectory">Экземпляр настроенного движка хранилища</param>
        /// <param name="fileEntity">Представление файла</param>
        public FileDescriptorHashedDirectoryBased(HashedDirectory hashedDirectory, IFileEntity fileEntity) {
            _hashedDirectoryStorage = hashedDirectory;
            FileEntity = fileEntity;
        }
        /// <summary>
        ///     Получение потока до файла
        /// </summary>
        /// <param name="access">Уровень доступа до потока</param>
        /// <returns>Поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _hashedDirectoryStorage.Open(FileEntity.Path);
        }
    }
}
