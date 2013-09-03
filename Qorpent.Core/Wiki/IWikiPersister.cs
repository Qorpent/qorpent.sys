using System;
using System.Collections.Generic;

namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс, идентичный IWikiSource, но отвечающий за сохранение в хранилище
	/// </summary>
	public interface IWikiPersister {
		/// <summary>
		/// Возвращает полностью загруженные страницы Wiki
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Get(params string[] codes);

		/// <summary>
		/// Возвращает страницы, только с загруженным признаком хранения в БД
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Exists(params string[] codes);

        /// <summary>
        ///     Возвращает страницу Wiki. Если версия не указана — последнюю актуальную
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        WikiPage GetWikiPageByVersion(string code, string version);

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
		/// Поиск страниц по маске 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> FindPages(string search);

		/// <summary>
		/// Поиск файлов по маске
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IEnumerable<WikiBinary> FindBinaries(string search);

		/// <summary>
		/// Возвращает версию файла
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		DateTime GetBinaryVersion(string code);
		/// <summary>
		/// Возвращает версию страницы
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		DateTime GetPageVersion(string code);

	    /// <summary>
	    ///     Создание версии страницы (копии последней с комментарием)
	    /// </summary>
	    /// <param name="code">Код страницы</param>
	    /// <param name="comment">Комментарий</param>
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
	    bool ReleaseLock(string code);
	}
}