using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Json {
	/// <summary>
	/// Объекты JSON
	/// </summary>
	public class JsonObject : JsonItem {
		/// <summary>
		/// Значения массива
		/// </summary>
		public List<JsonTuple> Properties = new List<JsonTuple>();
		/// <summary>
		/// Возвращает наличие свойства
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name) {
			return Properties.Any(_ => _.Name.Value == name);
		}
		/// <summary>
		/// Возвращает значение свойства по коду
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public JsonItem this[string name] {
			get { 
				if (!Contains(name)) return null;
				return Properties.First(_ => _.Name.Value == name).Value;
			}
			set {
				var property = Properties.FirstOrDefault (_ => _.Name.Value == name);
				if (null == property) {
					property = new JsonTuple{Name = new JsonValue(JsonTokenType.String,name)};
					Properties.Add(property);
				}
				property.Value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public override string ToString(bool format) {
			var sb = new StringBuilder();
			if (format) sb.Append(" ");
			sb.Append("{");
			
			foreach (var p in Properties) {
				if (format)
				{
					sb.Append(Environment.NewLine);
					for (var i = 1; i < Level; i++)
					{
						sb.Append("\t");
					}
				}

				
				sb.Append(p.Name.ToString(format));
				if (format) sb.Append(" ");
				sb.Append(":");
				if (format) sb.Append(" ");
				sb.Append(p.Value.ToString(format));
				sb.Append(",");
			}
			if (format)
			{
				sb.Append(Environment.NewLine);
				for (var i = 1; i < Level; i++)
				{
					sb.Append("\t");
				}
				
				
			}
			sb.Append("}");
			return sb.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		public override XElement WriteToXml(XElement current) {
			current = current ?? new XElement("object");
			// простые значения записываем атрибуты
			foreach (var p in Properties.Where(_ => _.Name.Type==JsonTokenType.Literal &&  _.Value is JsonValue)) {
				current.SetAttributeValue(p.Name.Value,p.Value.Value);
			}
			// простые но строковые имена пишем с контролем разрешенных имен
			foreach (var p in Properties.Where(_ => _.Name.Type == JsonTokenType.String && _.Value is JsonValue)) {
				if (IsLiteral(p.Name.Value)) {
					current.SetAttributeValue(p.Name.Value, p.Value.Value);	
				}else if (IsNumber(p.Name.Value)) {
					var e = new XElement("item", new XAttribute("idx", p.Name.Value), new XText(p.Value.Value));
					current.Add(e);
				}
				else {
					var e = new XElement("item", new XAttribute("name", p.Name.Value), new XText(p.Value.Value));
					current.Add(e);
				}
				
			}
			//массив пишем как вложенный элемент и передаем ему управление
			foreach (var p in Properties.Where(_ => _.Value is JsonArray)) {
				var arelement = new XElement(p.Name.Value, new XAttribute("__isarray", true));
				current.Add(arelement);
				p.Value.WriteToXml(arelement);
			}
			//а теперь пишем вложенные объекты
			foreach (var p in Properties.Where(_ => _.Value is JsonObject)) {
				var name = p.Name.Value;
				int idx = -1;
				if (name.All(_ => char.IsDigit(_))) {
					name = "item";
					idx = name.ToInt();
				}
				var arelement = new XElement(name);
				if (-1 != idx) {
					arelement.SetAttributeValue("idx",idx);
				}
				current.Add(arelement);
				p.Value.WriteToXml(arelement);
			}

			return current;
		}
	}
}