using System;
using System.Collections.Generic;
using System.Text;

namespace Qorpent.Dsl.Json {
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
	}
}