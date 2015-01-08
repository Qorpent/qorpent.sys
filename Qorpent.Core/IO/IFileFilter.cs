namespace Qorpent.IO {
    /// <summary>
    /// Фильтр, преобразующий файл при копировании
    /// </summary>
    public interface IFileFilter {
        /// <summary>
        /// 
        /// </summary>
        void Convert(string path);
    }
}