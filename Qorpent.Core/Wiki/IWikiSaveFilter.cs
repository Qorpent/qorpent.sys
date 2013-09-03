using Qorpent.Model;

namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс для фильтрации страницы при сохранении
	/// </summary>
	public interface IWikiSaveFilter:IWithIndex {
		/// <summary>
		/// Выполняет фильтрацию страницы перед сохранением
		/// </summary>
		/// <param name="page"></param>
		void Execute(WikiPage page);
	}
}