using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Действие получения страницы Wiki
	/// </summary>
	[Action("wiki.save", Help = "Сохранить правки в страницу", Role = "DOCWRITER")]
	public class WikiSaveAction : WikiActionBase
	{
		/// <summary>
		/// Код или коды страниц, которые требуется проверить
		/// </summary>
		[Bind(Required = true)]
		public string Code;
		/// <summary>
		/// Новый текст страницы
		/// </summary>
		[Bind] public string Text;

		/// <summary>
		/// Возвращает страницы Wiki по запросу
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			var page = new WikiPage {Code = Code, Text = Text};
			foreach (var parameter in Context.Parameters) {
				if (parameter.Key.ToUpper() != "CODE" && parameter.Key.ToUpper() != "TEXT") {
					page.Propeties[parameter.Key] = parameter.Value;
				}
			}
			WikiSource.Save(page);
			return WikiSource.Get(Code).First();
		}
	}
}