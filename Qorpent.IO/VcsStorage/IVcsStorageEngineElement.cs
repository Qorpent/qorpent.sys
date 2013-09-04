using System.IO;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Низкоуровневое представление элемента
    /// </summary>
    public interface IVcsStorageEngineElement {
        /// <summary>
        ///     Дескриптор файла
        /// </summary>
        IVcsStorageElementDescriptor Descriptor { get; }
        /// <summary>
        ///     Поток для взаимодействия с элементом
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        ///     Описывает возможности взаимодействия с потоком
        /// </summary>
        FileAccess StreamAccess { get; }
    }
}
