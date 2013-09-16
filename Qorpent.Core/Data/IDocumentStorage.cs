using System.Xml.Linq;

namespace Qorpent.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface IDocumentStorage {
        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        XElement ExecuteQuery(string query, DocumentStorageOptions options = null);

        /// <summary>
        /// Установить контекст работы
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        IDocumentStorage SetContext(string database, string collection);
    }
}