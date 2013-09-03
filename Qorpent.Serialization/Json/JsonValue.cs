using System;
using System.Xml.Linq;

namespace Qorpent.Json {
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
			Type = JsonTokenType.String;
			Value = value;
		}
		/// <summary>
		/// Полный конструктор
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public JsonValue(JsonTokenType type, string value) {
			Type = type;
			Value = value;
		}
		/// <summary>
		/// Создание литерала
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue Literal(string value) {
			return new JsonValue(JsonTokenType.Literal, value);
		}

		/// <summary>
		/// Создание строки
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue String(string value)
		{
			return new JsonValue(JsonTokenType.String, value);
		}

		/// <summary>
		/// Создание строки
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static JsonValue Number(string value)
		{
			return new JsonValue(JsonTokenType.Number, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public override string ToString(bool format) {
			if (JsonTokenType.Null == Type) return "null";
			if (JsonTokenType.Bool == Type) return Value=="false"?"false":"true";
			if (JsonTokenType.Literal == Type) return Value;
			if (JsonTokenType.Number == Type) return Value;
			if (JsonTokenType.String == Type)
				return "\"" + Value.Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")+"\"";
			throw new Exception("cannot to string " + Type);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		public override XElement WriteToXml(XElement current) {
			current = current ?? new XElement("value");
			current.Add(new XText(Value));
			return current;
		}
	}
}