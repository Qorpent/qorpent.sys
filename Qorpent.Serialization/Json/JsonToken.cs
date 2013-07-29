using Qorpent.Json;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// 
	/// </summary>
	public class JsonToken {
		/// <summary>
		/// 
		/// </summary>
		public JsonToken(JsonTokenType type, string value = null) {
			Type = type;
			Value = value;
		}
		/// <summary>
		/// 
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Type + ":" + Value;
		}
		/// <summary>
		/// Тип
		/// </summary>
		public JsonTokenType Type;
		/// <summary>
		/// Значение
		/// </summary>
		public string Value;

		private static readonly JsonToken _comma = new JsonToken(JsonTokenType.Comma,",");
		private static readonly JsonToken _colon = new JsonToken(JsonTokenType.Colon,":");
		private static readonly JsonToken _open = new JsonToken(JsonTokenType.BeginObject,"{");
		private static readonly JsonToken _close = new JsonToken(JsonTokenType.CloseObject,"}");
		private static readonly JsonToken _opena = new JsonToken(JsonTokenType.OpenArray,"[");
		private static readonly JsonToken _closea = new JsonToken(JsonTokenType.CloseArray,"]");
		private static readonly JsonToken _true = new JsonToken(JsonTokenType.Bool,"true");
		private static readonly JsonToken _false = new JsonToken(JsonTokenType.Bool,"false");
		private static readonly JsonToken _null = new JsonToken(JsonTokenType.Null,"null");
		/// <summary>
		/// Запятая
		/// </summary>
		public static JsonToken Comma {
			get { return _comma; }
		}
		/// <summary>
		/// Двоеточие
		/// </summary>
		public static JsonToken Colon
		{
			get { return _colon; }
		}
		
		/// <summary>
		/// Открытие объекта
		/// </summary>
		public static JsonToken Open
		{
			get { return _open; }
		}

		/// <summary>
		/// Закрытие объекта
		/// </summary>
		public static JsonToken Close
		{
			get { return _close; }
		}

		/// <summary>
		/// Открытие массива
		/// </summary>
		public static JsonToken OpenArray
		{
			get { return _opena; }
		}


		/// <summary>
		/// Закрытие массива
		/// </summary>
		public static JsonToken CloseArray
		{
			get { return _closea; }
		}


		/// <summary>
		/// Литерал False
		/// </summary>
		public static JsonToken False
		{
			get { return _false; }
		}

		/// <summary>
		/// Литерал True
		/// </summary>
		public static JsonToken True
		{
			get { return _true; }
		}

		/// <summary>
		/// Литерал null
		/// </summary>
		public static JsonToken Null
		{
			get { return _null; }
		}
		/// <summary>
		/// Создает строку
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static JsonToken String(string s) {
			return new JsonToken(JsonTokenType.String, s);
		}

		/// <summary>
		/// Создает число
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static JsonToken Num(decimal s)
		{
			return new JsonToken(JsonTokenType.Number, s.ToString());
		}

		/// <summary>
		/// Создает литрал
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static JsonToken Lit(string s)
		{
			return new JsonToken(JsonTokenType.Literal, s);
		}
	}
}