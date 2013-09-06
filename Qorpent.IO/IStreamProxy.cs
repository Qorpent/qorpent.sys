using System.IO;
using System.Threading.Tasks;

namespace Qorpent.IO {
    /// <summary>
    ///     Интерфейс описывает класс, проксирующий один поток на чтение
    ///     в несколько потоков на запись
    /// </summary>
    public interface IStreamProxy {
        /// <summary>
        ///     Проксирование
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="targets">Целевые потоки</param>
        /// <returns>Количество проксированных байт</returns>
        int Proxy(Stream source, params Stream[] targets);
        /// <summary>
        ///     Проксирование потока в файлы
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="paths">Полные пути до целевых файлов</param>
        /// <returns>Количество проксированных байт</returns>
        int Proxy(Stream source, params string[] paths);
        /// <summary>
        ///     Асинхронное проксирование
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="targets">Целевые потоки</param>
        /// <returns>Количество проксированных байт</returns>
        Task<int> ProxyAsync(Stream source, params Stream[] targets);
        /// <summary>
        ///     Асинхронное проксирование потока в файлы
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="paths">Полные пути до целевых файлов</param>
        /// <returns>Количество проксированных байт</returns>
        Task<int> ProxyAsync(Stream source, params string[] paths);
    }
}
