using System;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Интерфейс описания транзакции
    /// </summary>
    class VcsStorageTransaction {
        /// <summary>
        ///     Полный путь к файлу от корня хранилища
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        ///     Дата произведения операции
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        ///     Идентификатор коммита
        /// </summary>
        public string Commit { get; set; }
        /// <summary>
        ///     Тип транзакции
        /// </summary>
        public VcsStorageTransactionType Type { get; set; }
    }
}
