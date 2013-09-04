
namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутренний элемент хранилища
    /// </summary>
    public interface IVcsStorageElement {
        /// <summary>
        ///     Имя файла в хранилище
        /// </summary>
        string Filename { get; set; }
        /// <summary>
        ///     Внутренний идентификатор коммита
        /// </summary>
        string Commit { get; set; }
    }
}