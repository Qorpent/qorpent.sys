using Qorpent.IO;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Интерфейс загрузчика скриптов
    /// </summary>
    public interface ILoadService {
        /// <summary>
        /// Метод синхронизации на чтение/доступ к файлам скрипта
        /// </summary>
        void Synchronize();

        /// <summary>
        /// Асинхронно форсирует компиляцию скриптов
        /// </summary>
        void Compile();

        /// <summary>
        /// Получает имя ресурса
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pathtype"></param>
        /// <returns></returns>
        string GetFileName(LoadLevel level, FileSearchResultType pathtype = FileSearchResultType.FullPath);
    }
}