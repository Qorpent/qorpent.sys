using System.IO;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на хранилище DirtyVersion
    /// </summary>
    public class DirtyVersionBasedFileDescriptor : IGeneralFileDescriptor {
        /// <summary>
        ///     Дескриптор файла, основанный на хранилище DirtyVersion
        /// </summary>
        private readonly IDirtyVersionStorage _dirtyVersionStorage;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity FileEntity { get; private set; }
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="access">Уровень доступа</param>
        /// <returns>поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _dirtyVersionStorage.Open(FileEntity.Path, FileEntity.Version);
        }
        /// <summary>
        ///     Дескриптор файла, основанный на хранилище DirtyVersion
        /// </summary>
        public DirtyVersionBasedFileDescriptor(IDirtyVersionStorage storage, Commit commit) {
            _dirtyVersionStorage = storage;

            FileEntity = new FileEntity {
                Path = commit.MappingInfo.Name,
                Owner = (commit.Author != null) ? (commit.Author.Commiter) : (null),
                Version = commit.Hash
            };
        }
    }
}
