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
        /// <param name="file"></param>
        /// <param name="stream">поток-источник</param>
        public IGeneralFileDescriptor Set(IFileEntity file, Stream stream) {
            return new DirtyVersionBasedFileDescriptor(
                DirtyVersionStorage,
                DirtyVersionStorage.Save(file.Path, stream, file.Version)
            );
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IGeneralFileDescriptor Get(IFileEntity file) {
            return new DirtyVersionBasedFileDescriptor(
                DirtyVersionStorage,
                new Commit {
                    MappingInfo = new MappingInfo {Name = file.Path},
                    Hash = file.Version
                }
            );
        }
        /// <summary>
        ///     Возвращает класс текущего хранилища
        /// </summary>
        /// <returns></returns>
        public object GetStorage() {
            return DirtyVersionStorage;
        }
    }
}
