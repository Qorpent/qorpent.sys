using Qorpent.Model;

namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс для формирования шаблонной страницы при отсутствии сохраненной
	/// </summary>
	public interface IWikiEmptyFilter:IWithIndex {
		/// <summary>
		/// Заполняет пустую несохраненную таблицу шаблоном
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="page"></param>
		void Execute(IWikiSource storage, WikiPage page);
	}
}