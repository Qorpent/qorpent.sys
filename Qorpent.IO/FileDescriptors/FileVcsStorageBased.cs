<<<<<<< HEAD:Qorpent.IO/FileDescriptors/FileDescriptorVcsStorageBased.cs
﻿using Qorpent.IO.VcsStorage;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на VcsStorage
    /// </summary>
    public class FileDescriptorVcsStorageBased : IGeneralFileDescriptor {
        /// <summary>
        ///     Внутренний экземпляр перзистера
        /// </summary>
        private readonly IVcsStoragePersister _vcsStoragePersister;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity FileEntity { get; private set; }
        /// <summary>
        ///     Дескриптор файла, основанный на VcsStorage
        /// </summary>
        /// <param name="fileEntity">Представление файла</param>
        /// <param name="persister">Экземпляр перзистера</param>
        public FileDescriptorVcsStorageBased(IFileEntity fileEntity, IVcsStoragePersister persister) {
            FileEntity = fileEntity;
            _vcsStoragePersister = persister;
        }
        /// <summary>
        ///     Получение потока для работы с файлом
        /// </summary>
        /// <param name="access">Параметер доступа до файла</param>
        /// <returns>Поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _vcsStoragePersister.Pick(new VcsCommit {File = FileEntity});
        }
    }
}
=======
﻿using Qorpent.IO.VcsStorage;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    ///     Дескриптор файла, основанный на VcsStorage
    /// </summary>
    public class FileVcsStorageBased : IFile {
        /// <summary>
        ///     Внутренний экземпляр перзистера
        /// </summary>
        private readonly IVcsStoragePersister _vcsStoragePersister;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileDescriptor Descriptor { get; private set; }
        /// <summary>
        ///     Дескриптор файла, основанный на VcsStorage
        /// </summary>
        /// <param name="fileDescriptor">Представление файла</param>
        /// <param name="persister">Экземпляр перзистера</param>
        public FileVcsStorageBased(IFileDescriptor fileDescriptor, IVcsStoragePersister persister) {
            Descriptor = fileDescriptor;
            _vcsStoragePersister = persister;
        }
        /// <summary>
        ///     Получение потока для работы с файлом
        /// </summary>
        /// <param name="access">Параметер доступа до файла</param>
        /// <returns>Поток до файла</returns>
        public Stream GetStream(FileAccess access) {
            return _vcsStoragePersister.Pick(new VcsCommit {File = Descriptor});
        }
    }
}
>>>>>>> origin/master:Qorpent.IO/FileDescriptors/FileVcsStorageBased.cs
