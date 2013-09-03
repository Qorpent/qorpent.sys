using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qorpent.Serialization
{
	/// <summary>
	/// Интерфейс класса очистки контента
	/// </summary>
	public interface IContentCleaner {
		/// <summary>
		/// Очищает переданный контент до совместимости с XML
		/// </summary>
		/// <param name="content"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		XElement CleanContent(string content,  ContentCleanerOptions options = null);
	}
}
