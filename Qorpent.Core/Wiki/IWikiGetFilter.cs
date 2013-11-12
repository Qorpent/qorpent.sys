using Qorpent.Model;

namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс расширения для обработки страницы перед выдачей
	/// </summary>
	public interface IWikiGetFilter:IWithIndex {
		/// <summary>
		/// Выполняет фильтрацию страницы перед отправкой пользователю
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="page"></param>
		/// <param name="usage">Вариант использования</param>
		void Execute(IWikiSource storage,  WikiPage page, string usage);
	}
}