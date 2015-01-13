using System.Xml.Linq;

namespace Qorpent.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface IDocumentStorage {
		/// <summary>
		///		Определяет текущий контекст
		/// </summary>
		/// <returns>Контекст</returns>
	    IContext GetContext();
        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        XElement ExecuteQuery(string query, DocumentStorageOptions options = null);
		/// <summary>
		///		Установка контекста работа
		/// </summary>
		/// <param name="context">Контекст</param>
		/// <returns>Замыкание на <see cref="IDocumentStorage"/></returns>
	    IDocumentStorage SetContext(IContext context);
    }
}