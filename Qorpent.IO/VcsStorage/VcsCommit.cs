namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутреннее представление коммита
    /// </summary>
    public class VcsCommit {
        private string _branch;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileEntity File { get; set; }
        /// <summary>
        ///     Код коммита
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///     Имя бранча
        /// </summary>
        public string Branch {
            get { return _branch ?? (_branch = VcsStorageDefaults.DefaultBranch); }
            set { _branch = value; }
        }
    }
}