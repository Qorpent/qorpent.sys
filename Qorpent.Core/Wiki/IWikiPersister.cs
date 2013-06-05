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
	}
}