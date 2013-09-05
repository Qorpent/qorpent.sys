using System;
using System.IO;

namespace Qorpent.IO {
    /// <summary>
    /// 
    /// </summary>
    class FsBasedFileDescriptor : IGeneralFileDescriptor {
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

            return File.Open(FileEntity.Path, FileMode.Open, access);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowedAccess"></param>
        /// <param name="fileEntity"></param>
        public FsBasedFileDescriptor(FileAccess allowedAccess, FileEntity fileEntity) {
            AllowedAccess = allowedAccess;
            FileEntity = fileEntity;
        }
    }
}
