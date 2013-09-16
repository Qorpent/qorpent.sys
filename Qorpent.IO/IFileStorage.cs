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
        IGeneralFileDescriptor Set(IFileEntity file, Stream stream);
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IGeneralFileDescriptor Get(IFileEntity file);
        /// <summary>
        ///     Возвращает класс текущего хранилища текущее хранилища
        /// </summary>
        /// <returns></returns>
        object GetStorage();
    }
}
