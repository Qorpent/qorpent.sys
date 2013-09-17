using System.Collections.Generic;
using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     Интерфейс движка хранилища
    /// </summary>
    public interface IFileStorage {
        /// <summary>
        ///     Поддерживаемый функционал
        /// </summary>
        FileStorageAbilities Abilities { get; }
        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream">поток-источник</param>
        IFile Set(IFileDescriptor file, Stream stream);
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IFile Get(IFileDescriptor file);
        /// <summary>
        /// Возвращает перечисление файлов в хранилище
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IEnumerable<IFile> EnumerateFiles(FileSearchOptions options = null);

        /// <summary>
        ///     Возвращает класс текущего хранилища текущее хранилища
        /// </summary>
        /// <returns></returns>
        object GetUnderlinedStorage();
    }
}

