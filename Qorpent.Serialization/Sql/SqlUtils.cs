using Microsoft.SqlServer.Server;
using Qorpent.Json;

namespace Qorpent.Sql {
	/// <summary>
	///		Набор утилит для SQL
	/// </summary>
	public class SqlUtils {
		/// <summary>
		///		Преобразует JSON в XML
		/// </summary>
		/// <param name="json">Исходный JSON</param>
		/// <returns>Эквивалентный XML</returns>
		[SqlFunction]
		public static string JsonToXml(string json) {
			return new JsonParser().ParseXml(json).ToString();
		}
	}
}
