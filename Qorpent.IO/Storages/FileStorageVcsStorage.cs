using System.IO;
using Qorpent.IO.FileDescriptors;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Хранилище файлов, основанное на VcsStorage
    /// </summary>
    public class FileStorageVcsStorage : IFileStorageExtended {
        /// <summary>
        ///     Внутренний экземпляр класса хранилища
        /// </summary>
        private IVcsStoragePersister VcsStoragePersister { get; set; }
        /// <summary>
        ///     Поддерживаемый функционал
        /// </summary>
        public FileStorageAbilities Abilities { get; private set; }
        /// <summary>
        ///     Хранилище файлов, основанное на VcsStorage
        /// </summary>
        public FileStorageVcsStorage(IFileStorage vcsStorageEngine) {
            VcsStoragePersister = new VcsStoragePersister(vcsStorageEngine);
        }
        /// <summary>
        ///     Запись файла в хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток-источник</param>
        /// <returns></returns>
        public IGeneralFileDescriptor Set(IFileEntity file, Stream stream) {
            return new FileDescriptorVcsStorageBased(
                VcsStoragePersister.Commit(new VcsCommit { File = file }, stream).File,
                VcsStoragePersister
            );
        }
        /// <summary>
        ///     Получение файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns>Дескриптор файла</returns>
        public IGeneralFileDescriptor Get(IFileEntity file) {
            return new FileDescriptorVcsStorageBased(file, VcsStoragePersister);
        }
        /// <summary>
        ///     Производит удаление файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        public void Del(IFileEntity file) {
            VcsStoragePersister.Remove(file);
        }
        /// <summary>
        ///     Получение нативного движка хранилища
        /// </summary>
        /// <returns></returns>
        public object GetStorage() {
            return VcsStoragePersister;
        }
    }
}
