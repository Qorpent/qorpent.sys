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

			foreach (var p in Values)
			{
				if (format)
				{
					sb.Append(Environment.NewLine);
					for (var i = 1; i < Level; i++)
					{
						sb.Append("\t");
					}
				}


				sb.Append(p.ToString(format));		
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
			sb.Append("]");
			return sb.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		public override XElement WriteToXml(XElement current) {
			current = current ?? new XElement("array");
			for(var i=0;i<Values.Count;i++) {
				var v = Values[i];
				var e = new XElement("item", new XAttribute("idx", i));
				current.Add(e);
				v.WriteToXml(e);
			}
			return current;
		}
	}
}