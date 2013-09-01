using System.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
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

	/// <summary>
	/// Базовый класс действий при работе с Wiki
	/// </summary>
	[Action("wiki.find", Help = "Осуществляет поиск объектов")]
	public class WikiFind : WikiActionBase {
		/// <summary>
		/// Регекс поиска
		/// </summary>
		[Bind] public string Search { get; set; }
		/// <summary>
		/// Начальный индекс поисковой страницы
		/// </summary>
		[Bind(Default = -1)]public int Start { get; set; }
		/// <summary>
		/// Число результатов поиска
		/// </summary>	
		[Bind(Default = -1)]public int Count { get; set; }

		/// <summary>
		/// Число результатов поиска
		/// </summary>	
		[Bind(Default = true)]public bool Files { get; set; }
		/// <summary>
		/// Число результатов поиска
		/// </summary>	
		[Bind(Default = true)]public bool Pages { get; set; }

		/// <summary>
		/// Возвращает найденные дескрипторы
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			var type = WikiObjectType.None;
			if (Files) type = type | WikiObjectType.File;
			if (Pages) type = type | WikiObjectType.Page;
            if (string.IsNullOrEmpty(Search)) Search = "*";
			return WikiSource.Find(Search, Start, Count, type).ToArray()
			;
		}
		
	}
}