using System.Collections.Generic;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Объекты JSON
	/// </summary>
	public class JsonObject : JsonItem {
		/// <summary>
		/// Значения массива
		/// </summary>
		public List<JsonTuple> Properties = new List<JsonTuple>();
		
	}
}