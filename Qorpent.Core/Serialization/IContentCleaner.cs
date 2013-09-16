using System.Xml.Linq;

namespace Qorpent.Serialization
{
	/// <summary>
	/// Интерфейс класса очистки контента
	/// </summary>
	/// <remarks>
	/// Интерфейс используется для преобразования плохо отформатированного HTML для
	/// совместимости с XML, стандартной реализации в Qorpent.Sys не содержится
	/// пример реализации можно увидеть в Qorpent.Integration.Tidy
	/// </remarks>
	public interface IContentCleaner {
		/// <summary>
		/// Очищает переданный контент до совместимости с XML
		/// </summary>
		/// <param name="content">Строка HTML или иного неверно сформатированного XML</param>
		/// <param name="options">Опции очистки HTML</param>
		/// <returns></returns>
		XElement CleanContent(string content,  ContentCleanerOptions options = null);
	}
}
