using System;
using System.Collections.Generic;

namespace Qorpent.Wiki
{
	/// <summary>
	/// Интерфейс, описывающий простеший репозиторий Wiki
	/// </summary>
	public interface IWikiSource {
		/// <summary>
		/// Возвращает полностью загруженные страницы Wiki
		/// </summary>
		/// <param name="usage">Варинат использования </param>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Get(string usage, params string[] codes);
		/// <summary>
		/// Возвращает страницы, только с загруженным признаком хранения в БД
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Exists(params string[] codes);
		/// <summary>
		/// Метод сохранения изменений в страницу
		/// </summary>
		/// <param name="pages"></param>
		bool Save(params WikiPage[] pages);

		/// <summary>
		/// Сохраняет в Wiki файл с указанным кодом
		/// </summary>
	
		void SaveBinary(WikiBinary binary);

		/// <summary>
		/// Загружает бинарный контент
		/// </summary>
		/// <param name="code"></param>
		/// <param name="withData">Флаг, что требуется подгрузка бинарных данных</param>
		/// <returns></returns>
		WikiBinary LoadBinary(string code, bool withData = true);

		/// <summary>
		/// Поиск объектов Wiki
		/// </summary>
		/// <param name="search"></param>
		/// <param name="count"></param>
		/// <param name="types"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		IEnumerable<WikiObjectDescriptor> Find(string search, int start=-1, int count=-1, WikiObjectType types = WikiObjectType.All);

		/// <summary>
		/// Возвращает версию объекта
		/// </summary>
		/// <param name="code"></param>
		/// <param name="objectType"></param>
		/// <returns></returns>
		DateTime GetVersion(string code, WikiObjectType objectType);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="code"></param>
	    /// <param name="version"></param>
	    /// <returns></returns>
	    WikiPage GetWikiPageByVersion(string code, string version);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="code"></param>
	    /// <param name="comment"></param>
	    /// <returns></returns>
	    object CreateVersion(string code, string comment);

        /// <summary>
        ///     Восстановление состояние страницы на момент определённой версии
        /// </summary>
        /// <param name="code">Код страницы</param>
        /// <param name="version">Идентификатор версии</param>
        /// <returns></returns>
        object RestoreVersion(string code, string version);

	    /// <summary>
	    ///     Возвращает список версий и первичную информацию о документе по коду
	    /// </summary>
	    /// <param name="code">Wiki page code</param>
	    /// <returns></returns>
	    IEnumerable<object> GetVersionsList(string code);

        /// <summary>
        ///     Установить блокировку
        /// </summary>
        /// <param name="code">Код страницы</param>
        /// <returns>Результат операции</returns>
        bool GetLock(string code);

        /// <summary>
        ///     Снять блокировку
        /// </summary>
        /// <param name="code">код страницы</param>
        bool Releaselock(string code);
	}
}
