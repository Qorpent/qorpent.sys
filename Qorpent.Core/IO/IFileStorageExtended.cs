
﻿namespace Qorpent.IO {
    /// <summary>
    ///     Расширенное хранилище с поддержкой удаления
    /// </summary>
    public interface IFileStorageExtended : IFileStorage {
        /// <summary>
        ///     Производит удаление файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        void Del(IFileDescriptor file);
    }
}
