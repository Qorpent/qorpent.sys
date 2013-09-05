using System.IO;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.FileDescriptors {
    class DirtyVersionBasedFileDescriptor : IGeneralFileDescriptor {
        /// <summary>
        ///     Хранилище
        /// </summary>
        private readonly IDirtyVersionStorage _dirtyVersionStorage;
        /// <summary>
        /// 
        /// </summary>
        public IFileEntity FileEntity { get; private set; }
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public Stream GetStream(FileAccess access) {
            return _dirtyVersionStorage.Open(FileEntity.Path, FileEntity.Version);
        }
        /// <summary>
        /// 
        /// </summary>
        public DirtyVersionBasedFileDescriptor(IDirtyVersionStorage storage, Commit commit) {
            _dirtyVersionStorage = storage;

            FileEntity = new FileEntity {
                Path = commit.MappingInfo.Name,
                Owner = commit.Author.Commiter,
                Version = commit.Hash
            };
        }
    }
}
