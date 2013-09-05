namespace Qorpent.IO.VcsStorage {
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
        /// <param name="engineElement">Представление элемента</param>
        void Set(IVcsStorageEngineElement engineElement);
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="descriptor">Дескриптор элемента</param>
        /// <returns></returns>
        IVcsStorageEngineElement Get(IVcsStorageElementDescriptor descriptor);
    }
}
