using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Config {	
	/// <summary>
	///		Источник XML-конфигураций
	/// </summary>
	public interface IXmlConfigSource {
		/// <summary>
		///		Получить единичный конфиг
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		XElement Get(string code);
		/// <summary>
		/// Получить набор конфигов по условию
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		IEnumerable<XElement> Find(string query);
	}
}
