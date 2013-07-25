using System.Collections.Generic;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Массив JSON
	/// </summary>
	public class JsonArray:JsonItem {
		/// <summary>
		/// Значения массива
		/// </summary>
		public List<JsonItem> Values = new List<JsonItem>();
	}
}