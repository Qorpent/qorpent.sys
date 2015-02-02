using System.Collections.Generic;

namespace Qorpent.IO {
    /// <summary>
    /// Интерфейс файлового кэша, завязанного на внешние источники
    /// </summary>
    public interface IFileCacheResolver {
        /// <summary>
        /// Директория локального кэша
        /// </summary>
        string Root { get; set; }

        /// <summary>
        /// Источники данных
        /// </summary>
        IList<IFileCacheSource> Sources { get; }

        /// <summary>
        /// Имя файла, возвращаемого при возникновении несуществующего результата (замена null)
        /// </summary>
        string Fallback { get; set; }

        /// <summary>
        /// Фильтры
        /// </summary>
        IList<IFileFilter> Filters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Resolve(string name, bool forceUpdate = false);
    }
}