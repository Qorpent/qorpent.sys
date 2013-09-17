using System.Collections.Generic;
using Qorpent.IO.DirtyVersion.Storage;
using Qorpent.IO.FileDescriptors;
using System.IO;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Класс файлового хранилища, основанный на HasedDirectory
    /// </summary>
    public class FileStorageHashedDirectory : IFileStorage {
        /// <summary>
        ///     Экземпляр класса нативного хранилища
        /// </summary>
        private readonly HashedDirectory _hashedDirectoryStorage;
        /// <summary>
        ///     Текущая рабочая директория
        /// </summary>
        public string WorkingDirectory { get; private set; }
        /// <summary>
        ///     Возможности хранилища
        /// </summary>
        public FileStorageAbilities Abilities { get; private set; }
        /// <summary>
        ///     Класс файлового хранилища, основанный на HasedDirectory
        /// </summary>
        /// <param name="workingDirectory">Рабочая директория</param>
        public FileStorageHashedDirectory(string workingDirectory) {
            Abilities = FileStorageAbilities.Persist;
            WorkingDirectory = workingDirectory;
            _hashedDirectoryStorage = new HashedDirectory(WorkingDirectory);
        }
        /// <summary>
        ///     Сохранение файла в хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток-источник</param>
        /// <returns>Дескриптор файла</returns>
        public IFile Set(IFileDescriptor file, Stream stream) {
            var record = _hashedDirectoryStorage.Write(file.Path, stream);
            return new FileHashedDirectoryBased(
                _hashedDirectoryStorage,
                new FileDescriptor {
                    Path = file.Path,
                    Filename = record.NameHash,
                    DateTime = record.LastWriteTime,
                    Version = record.DataHash
                }
            );
        }
        /// <summary>
        ///     Получение файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns>Дескриптор файла</returns>
        public IFile Get(IFileDescriptor file) {
            return new FileHashedDirectoryBased(_hashedDirectoryStorage, file);
        }

        /// <summary>
        /// Возвращает перечисление файлов в хранилище
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<IFile> EnumerateFiles(FileSearchOptions options = null) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Получение экземпляра класса нативного движка хранилища
        /// </summary>
        /// <returns>Настроенный на данное хранилище экземпляр HashedDirectory</returns>
        public object GetUnderlinedStorage() {
            return _hashedDirectoryStorage;
        }
    }
}
