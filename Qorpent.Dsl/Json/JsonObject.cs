using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qorpent.Dsl.Json {
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
					property = new JsonTuple{Name = new JsonValue(TType.Str,name)};
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
	}
}