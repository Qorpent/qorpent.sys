using System.IO;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на хранилище DirtyVersion
    /// </summary>
    public class FileDirtyVersionBased : IFile {
        /// <summary>
        ///     Дескриптор файла, основанный на хранилище DirtyVersion
        /// </summary>
        private readonly IDirtyVersionStorage _dirtyVersionStorage;
        /// <summary>
        ///     Коммит, соответутсвующий данному файлу
        /// </summary>
        public Commit Commit { get; private set; }
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileDescriptor Descriptor { get; private set; }
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="access">Уровень доступа</param>
        /// <returns>поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _dirtyVersionStorage.Open(Descriptor.Path, Descriptor.Version);
        }
        /// <summary>
        ///     Дескриптор файла, основанный на хранилище DirtyVersion
        /// </summary>
        public FileDirtyVersionBased(IDirtyVersionStorage storage, Commit commit) {
            _dirtyVersionStorage = storage;
            Commit = commit;
            Descriptor = new FileDescriptor {
                Path = commit.MappingInfo.Name,
                Owner = (commit.Author != null) ? (commit.Author.Commiter) : (null),
                Version = commit.Hash
            };
        }
    }
}
