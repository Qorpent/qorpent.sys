using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Serialization {
	/// <summary>
	///     Описывает компонент поиска элементов в XML-контенте
	/// </summary>
	public interface ISelector {
		/// <summary>
		///     Вы
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		IEnumerable<XElement> Select(XElement root, string query, SelectorLanguage language = SelectorLanguage.Auto);
	}
}