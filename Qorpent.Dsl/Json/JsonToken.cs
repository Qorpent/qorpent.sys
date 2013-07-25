namespace Qorpent.Dsl.Json {
	/// <summary>
	/// 
	/// </summary>
	public struct JsonToken {
		/// <summary>
		/// 
		/// </summary>
		public JsonToken(TType type, string value = null) {
			Type = type;
			Value = value;
		}
		/// <summary>
		/// Тип
		/// </summary>
		public TType Type;
		/// <summary>
		/// Значение
		/// </summary>
		public string Value;
	}
}