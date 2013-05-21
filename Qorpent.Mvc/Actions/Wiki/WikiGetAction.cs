using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Действие получения страницы Wiki
	/// </summary>
	[Action("wiki.get",Help = "Получить Wiki по заданным кодам")]
	public class WikiGetAction:WikiActionBase {
		/// <summary>
		/// Код или коды страниц, которые требуется получить
		/// </summary>
		[Bind(Required = true)] public string Code;
		/// <summary>
		/// Вариант использования
		/// </summary>
		[Bind] public string Usage;
		/// <summary>
		/// Возвращает страницы Wiki по запросу
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			return WikiSource.Get(Usage,Code.SmartSplit(false,true,',').ToArray()).ToArray();
		}
	}
}