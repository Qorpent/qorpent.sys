using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Selector.Implementations {
	/// <summary>
	///     Селектор для поддержки XPATH нотации селекторов
	/// </summary>
	[ContainerComponent(Lifestyle = Lifestyle.Transient, Name = "selector.xpath", ServiceType = typeof(ISelectorImpl))]
	public class XPathSelectorImpl : ISelectorImpl {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public IEnumerable<XElement> Select(XElement root, string query) {
			// так как элемент указанный в качестве root для запроса
			// может быть дочерним элементом документа,требуется проверить не начинается ли запрос от абсолютного
			// корня и если да, сделать го относительным
			if (query.StartsWith("/")) {
				query = "." + query;
			}
			// имеем только в виду что такой метод не годится
			// если вдруг будем работать с контентом, включающим пространства имен
			// в этом случае потребуется настройка менеджера пространств
			return root.XPathSelectElements(query);
		}
	}
}