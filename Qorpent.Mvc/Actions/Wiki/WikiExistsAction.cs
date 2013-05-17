using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// ƒействие получени€ страницы Wiki
	/// </summary>
	[Action("wiki.exists", Help = "ѕолучить наличие Wiki по заданным кодам")]
	public class WikiExistsAction : WikiActionBase
	{
		/// <summary>
		///  од или коды страниц, которые требуетс€ проверить
		/// </summary>
		[Bind(Required = true)]
		public string Code;
		/// <summary>
		/// ¬озвращает страницы Wiki по запросу
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess()
		{
			return WikiSource.Exists(Code.SmartSplit(false, true, ',').ToArray()).ToArray();
		}
	}
}