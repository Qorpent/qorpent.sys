<<<<<<< HEAD:Qorpent.IO/FileDescriptors/FileDescriptorFsBased.cs
﻿using System;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    /// 
    /// </summary>
    class FileDescriptorFsBased : IGeneralFileDescriptor {
        /// <summary>
        ///     Разрешённый уровень доступа
        /// </summary>
        public FileAccess AllowedAccess { get; private set; }
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity FileEntity { get; private set; }
        /// <summary>
        ///     Получить поток до файла
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public Stream GetStream(FileAccess access) {
            if (!AllowedAccess.HasFlag(access)) {
                throw new Exception("You haven't access to this file");
            }

            return File.Exists(FileEntity.Path) ? File.Open(FileEntity.Path, FileMode.Open, access) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowedAccess"></param>
        /// <param name="fileEntity"></param>
        public FileDescriptorFsBased(FileAccess allowedAccess, FileEntity fileEntity) {
            AllowedAccess = allowedAccess;
            FileEntity = fileEntity;
        }
    }
}
=======
﻿using System;
using System.IO;

namespace Qorpent.IO.FileDescriptors {
    /// <summary>
    /// 
    /// </summary>
    class FileFsBased : IFile {
        /// <summary>
        ///     Разрешённый уровень доступа
        /// </summary>
        public FileAccess AllowedAccess { get; private set; }
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileDescriptor Descriptor { get; private set; }
        /// <summary>
        ///     Получить поток до файла
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public Stream GetStream(FileAccess access) {
            if (!AllowedAccess.HasFlag(access)) {
                throw new Exception("You haven't access to this file");
            }

            return File.Exists(Descriptor.Path) ? File.Open(Descriptor.Path, FileMode.Open, access) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowedAccess"></param>
        /// <param name="fileDescriptor"></param>
        public FileFsBased(FileAccess allowedAccess, FileDescriptor fileDescriptor) {
            AllowedAccess = allowedAccess;
            Descriptor = fileDescriptor;
        }
    }
}
>>>>>>> origin/master:Qorpent.IO/FileDescriptors/FileFsBased.cs
