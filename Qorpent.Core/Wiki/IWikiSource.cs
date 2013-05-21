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
	}
}
