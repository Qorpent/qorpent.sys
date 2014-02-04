using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.BSharp{
	/// <summary>
	/// Интерфейс адаптации SQL - запросов в XML для BSharp
	/// </summary>
	public interface IBSharpSqlAdapter{
		/// <summary>
		/// Считывает набор данных и оборачивает в XML элемент с указанным именем, по умолчанию item - для dataset
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="query"></param>
		/// <param name="elementName"></param>
		/// <returns></returns>
		IEnumerable<XElement> ExecuteReader(string connection, string query, string elementName = "item");
	}
}