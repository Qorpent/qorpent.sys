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
		void Save(params WikiPage[] pages);

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
	}
}
