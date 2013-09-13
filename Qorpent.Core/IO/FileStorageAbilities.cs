namespace Qorpent.IO {
    /// <summary>
    ///     Поддерживаемый функционал
    /// </summary>
    public enum FileStorageAbilities {
        /// <summary>
        ///     Чистый фэйк
        /// </summary>
        Default,
        /// <summary>
        ///     Простое хранилище
        /// </summary>
        Persist,
        /// <summary>
        ///     Поддержка версионирования
        /// </summary>
        Versions,
        /// <summary>
        ///     Хранилище с поддержкой версионирования
        /// </summary>
        Vcs = Persist | Versions
    }
}