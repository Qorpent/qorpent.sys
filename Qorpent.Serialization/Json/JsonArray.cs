using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Json {
	/// <summary>
	/// Массив JSON
	/// </summary>
	public class JsonArray:JsonItem {
		/// <summary>
		/// Значения массива
		/// </summary>
		public List<JsonItem> Values = new List<JsonItem>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public override string ToString(bool format) {
			var sb = new StringBuilder();
			if (format) sb.Append(" ");
			sb.Append("[");
		    var pi = 0;
			foreach (var p in Values) {
			    pi++;
				if (format)
				{
					sb.Append(Environment.NewLine);
					for (var i = 1; i < Level; i++)
					{
						sb.Append("\t");
					}
				}


				sb.Append(p.ToString(format));
			    if (pi != Values.Count) {
			        sb.Append(",");
			    }
			}
			if (format)
			{
				sb.Append(Environment.NewLine);
				for (var i = 1; i < Level; i++)
				{
					sb.Append("\t");
				}
				

			}
			sb.Append("]");
			return sb.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		public override XElement WriteToXml(XElement current) {
			current = current ?? new XElement("array");
            current.SetAttributeValue(JsonTypeAttributeName,"array");
			for(var i=0;i<Values.Count;i++) {
				var v = Values[i];
				var e = new XElement("item", new XAttribute("idx", i), new XAttribute(JsonTypeAttributeName,v.Type));
				current.Add(e);
				v.WriteToXml(e);
			}
			return current;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<JsonItem> CollectAllValues() {
			foreach (var jsonItem in Values) {
				if (jsonItem is JsonValue) yield return  jsonItem;
				else
					foreach (var collectAllValue in jsonItem.CollectAllValues()) {
						yield return collectAllValue;
					}

			}
		}
	}
}