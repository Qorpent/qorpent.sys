using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Serialization {
	/// <summary>
	///     Интерфейс имплементации селектора для основных языков
	/// </summary>
	public interface ISelectorImpl {
		/// <summary>
		///     Выполняет
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		IEnumerable<XElement> Select(XElement root, string query);
	}
}