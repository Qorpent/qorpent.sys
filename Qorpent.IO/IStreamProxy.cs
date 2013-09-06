using System.IO;

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
    }
}
