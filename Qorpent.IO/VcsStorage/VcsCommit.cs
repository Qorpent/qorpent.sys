namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутреннее представление коммита
    /// </summary>
    public class VcsCommit {
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity File { get; set; }
        /// <summary>
        ///     Код коммита
        /// </summary>
        public string Code { get; set; }
    }
}