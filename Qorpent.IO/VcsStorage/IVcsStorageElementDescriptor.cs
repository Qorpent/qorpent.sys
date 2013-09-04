namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Дескриптор элемента
    /// </summary>
    public interface IVcsStorageElementDescriptor {
        /// <summary>
        ///     Директория расположения файла относительно корня
        /// </summary>
        string RelativeDirectory { get; }
        /// <summary>
        ///     Имя файла в директории
        /// </summary>
        string Filename { get; }
    }
}