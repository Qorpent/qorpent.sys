using System.Collections.Generic;
using System.IO;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;
using Qorpent.IO.FileDescriptors;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Хранилище файлов, основанное на DirtyVersion
    /// </summary>
    public class FileStorageDirtyVersion : IFileStorage {
        /// <summary>
        ///     Поддерживаемый функционал
        /// </summary>
        public FileStorageAbilities Abilities { get; private set; }
        /// <summary>
        ///     Класс хранилища
        /// </summary>
        private IDirtyVersionStorage DirtyVersionStorage { get; set; }
        /// <summary>
        ///     Хранилище файлов, основанное на DirtyVersion
        /// </summary>
        public FileStorageDirtyVersion(string workingDirectory) {
            Abilities = FileStorageAbilities.Vcs;
            DirtyVersionStorage = new DirtyVersionStorage(workingDirectory);
        }
        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток-источник</param>
        public IFile Set(IFileDescriptor file, Stream stream) {
            return new FileDirtyVersionBased(
                DirtyVersionStorage,
                DirtyVersionStorage.Save(file.Path, stream)
            );
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns>Дескриптор файла</returns>
        public IFile Get(IFileDescriptor file) {
            return new FileDirtyVersionBased(
                DirtyVersionStorage,
                new Commit {
                    MappingInfo = new MappingInfo {Name = file.Path},
                    Hash = file.Version
                }
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<IFile> EnumerateFiles(FileSearchOptions options = null) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Возвращает класс текущего хранилища
        /// </summary>
        /// <returns></returns>
        public object GetUnderlinedStorage() {
            return DirtyVersionStorage;
        }
    }
}