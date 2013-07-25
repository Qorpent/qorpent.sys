namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Минимальное значение JSON
	/// </summary>
	public class JsonValue:JsonItem {
		/// <summary>
		/// Простое создание строки
		/// </summary>
		/// <param name="value"></param>
		public JsonValue(string value)
		{
			Type = TType.Str;
			Value = value;
		}
		/// <summary>
		/// Полный конструктор
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public JsonValue(TType type, string value) {
			Type = type;
			Value = value;
		}
		/// <summary>
		/// Создание литерала
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue Literal(string value) {
			return new JsonValue(TType.Lit, value);
		}

		/// <summary>
		/// Создание строки
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue String(string value)
		{
			return new JsonValue(TType.Str, value);
		}

		/// <summary>
		/// Создание строки
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue Number(string value)
		{
			return new JsonValue(TType.Num, value);
		}

		/// <summary>
		/// Тип значения
		/// </summary>
		public TType Type;
		/// <summary>
		/// Само значение
		/// </summary>
		public string Value;
	}
}