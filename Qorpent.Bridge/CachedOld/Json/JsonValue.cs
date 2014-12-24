using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.CachedOld.Json
{
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
	    /// Создает Json ээлемент из XML
	    /// </summary>
	    /// <param name="value"></param>
	    public JsonValue(XElement value) {
	        this.Type = JsonTokenType.String;
	        var type = value.Attr(JsonTypeAttributeName);
            if (!string.IsNullOrWhiteSpace(type)) {
                Type = (JsonTokenType) Enum.Parse(typeof (JsonTokenType), type, true);
            }
	        Value = value.Value;
	    }
        /// <summary>
        /// 
        /// </summary>
	    public JsonValue() {
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
		    var type = Type;
		    if (JsonTokenType.Auto == type) {
               type=  DetermineAutoType();
            }
			if (JsonTokenType.Null == type) return "null";
            if (JsonTokenType.Bool == type) return Value == "false" ? "false" : "true";
            if (JsonTokenType.Literal == type) return Value;
            if (JsonTokenType.Number == type) return Value;
            if (JsonTokenType.String == type)
            {
			    if (format) {
			        return "\"" + EscapeStringValue() + "\"";
			    }
			    else {
			        return Value;
			    }
			}
				
			throw new Exception("cannot to string " + Type);
		}

	    private JsonTokenType DetermineAutoType() {
            if (Value == null || Value == "null") return JsonTokenType.Null;
	        if (Value == "true" || Value== "false")return JsonTokenType.Bool;
	        if (Regex.IsMatch(Value, @"^-?\d+(\.\d+)?$")) return JsonTokenType.Number;
            return JsonTokenType.String;
	    }

	    private string EscapeStringValue() {
	        if (string.IsNullOrWhiteSpace(Value)) return string.Empty;
	        return Value.Replace("\\","\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
	    }

	    /// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		public override XElement WriteToXml(XElement current) {


			current = current ?? new XElement("value", new XAttribute(JsonTypeAttributeName,Type));
		    current.Value = "[REPLACE]";
		    var value = Escaper.Escape(ToString(false), EscapingType.XmlAttribute);
		    var result = current.ToString().Replace("[REPLACE]", value);
		    var resulte = XElement.Parse(result);
			if (null != current.Parent)
			{
				current.ReplaceWith(resulte);
			}
		    return resulte;
		}

		/// <summary>
		/// Перечисляет все значения
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<JsonItem> CollectAllValues() {

			yield return this;
		}
	}
}