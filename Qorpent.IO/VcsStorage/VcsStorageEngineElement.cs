using System.IO;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Низкоуровневое представление элемента
    /// </summary>
    class VcsStorageEngineElement : IVcsStorageEngineElement {
        /// <summary>
        ///     Дескриптор файла
        /// </summary>
        public IVcsStorageElementDescriptor Descriptor { get; set; }
        /// <summary>
        ///     Поток для взаимодействия с элементом
        /// </summary>
        public Stream Stream { get; set; }
        /// <summary>
        ///     Описывает возможности взаимодействия с потоком
        /// </summary>
        public FileAccess StreamAccess { get; set; }
    }
}
