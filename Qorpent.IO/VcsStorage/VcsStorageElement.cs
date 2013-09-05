
namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутренний элемент хранилища
    /// </summary>
    public class VcsStorageElement : IVcsStorageElement {
        /// <summary>
        ///     Имя файла в хранилище
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        ///     Внутренний идентификатор коммита
        /// </summary>
        public string Commit { get; set; }
    }
}
