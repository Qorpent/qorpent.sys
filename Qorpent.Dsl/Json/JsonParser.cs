namespace Qorpent.Dsl.Json
{
	/// <summary>
	/// Парсер JSON
	/// </summary>
	public class JsonParser
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
	}
}
