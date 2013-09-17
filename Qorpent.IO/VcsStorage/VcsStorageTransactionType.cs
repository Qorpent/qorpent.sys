namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Перечисление представляет тип транзакции
    /// </summary>
    enum VcsStorageTransactionType {
        /// <summary>
        ///     Неизвестная транзакция
        /// </summary>
        Undefined = 1,
        /// <summary>
        ///     Коммит в хранилище
        /// </summary>
        Commit = 2,
        /// <summary>
        ///     Удаление из хранилища
        /// </summary>
        Remove = 4,
        /// <summary>
        ///     Восстановление (коммит поверх)
        /// </summary>
        Revert = Commit
    }
}
