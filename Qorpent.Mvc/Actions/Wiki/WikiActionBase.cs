using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Базовый класс действий при работе с Wiki
	/// </summary>
	public abstract class WikiActionBase : ActionBase {
		/// <summary>
		/// Объект хранилища Wiki
		/// </summary>
		[Inject]public IWikiSource WikiSource { get; set; }
	}
}