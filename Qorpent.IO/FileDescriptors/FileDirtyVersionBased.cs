<<<<<<< HEAD:Qorpent.IO/FileDescriptors/FileDescriptorDirtyVersionBased.cs
﻿using System.IO;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на хранилище DirtyVersion
    /// </summary>
    public class FileDescriptorDirtyVersionBased : IGeneralFileDescriptor {
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
        public FileDescriptorDirtyVersionBased(IDirtyVersionStorage storage, Commit commit) {
            _dirtyVersionStorage = storage;

            FileEntity = new FileEntity {
                Path = commit.MappingInfo.Name,
                Owner = (commit.Author != null) ? (commit.Author.Commiter) : (null),
                Version = commit.Hash
            };
        }
    }
}
=======
﻿using System.IO;
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
>>>>>>> origin/master:Qorpent.IO/FileDescriptors/FileDirtyVersionBased.cs
