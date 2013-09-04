using System;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутренний экзепшен
    /// </summary>
    public class VcsStorageException : Exception {
        /// <summary>
        ///     Внутренний экзепшен
        /// </summary>
        /// <param name="message">Сообщение экзепшена</param>
        public VcsStorageException(string message) : base(message) {}
    }
}
