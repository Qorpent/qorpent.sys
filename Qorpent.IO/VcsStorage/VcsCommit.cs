namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутреннее представление коммита
    /// </summary>
    public class VcsCommit {
        private string _branch;
        private string _commiter;
        /// <summary>
        ///     Представление файла
        /// </summary>
        public IFileDescriptor File { get; set; }
        /// <summary>
        ///     Код коммита
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///     Автор коммита
        /// </summary>
        public string Commiter {
            get { return _commiter ?? (_commiter = System.Security.Principal.WindowsIdentity.GetCurrent().Name); }
        }
        /// <summary>
        ///     Имя бранча
        /// </summary>
        public string Branch {
            get { return _branch ?? (_branch = VcsStorageDefaults.DefaultBranch); }
            set { _branch = value; }
        }
    }
}