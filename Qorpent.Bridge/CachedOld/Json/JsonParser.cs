using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.IoC;

namespace Qorpent.CachedOld.Json
{
	/// <summary>
	/// Парсер JSON
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,Name="json.parser",ServiceType = typeof(IJsonParser))]
	[ContainerComponent(Lifestyle.Transient, Name = "json.xml.parser",ServiceType = typeof(ISpecialXmlParser))]
	public class JsonParser:IJsonParser,ISpecialXmlParser
	{
		/// <summary>
		/// Преобразует строку в Json объект
		/// </summary>
		/// <param name="jsonstring"></param>
		/// <returns></returns>
		public JsonItem Parse(string jsonstring) {
			var tokens = new Tokenizer().Tokenize(jsonstring);
			return new Lexer().Collect(tokens);
		}
		/// <summary>
		/// Реализация интерфейса альтернативного конвертера в XML
		/// </summary>
		/// <param name="srccode"></param>
		/// <returns></returns>
		public XElement ParseXml(string srccode) {
			return ((IJsonParser) this).Parse(srccode).WriteToXml();
		}

	}
}
