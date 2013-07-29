namespace Qorpent.Json
{
	/// <summary>
	/// Интерфейс парсера JSON
	/// </summary>
	public interface IJsonParser {
		/// <summary>
		/// Парсит исходный код в объект Json
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		JsonItem Parse(string code);
	}
}
