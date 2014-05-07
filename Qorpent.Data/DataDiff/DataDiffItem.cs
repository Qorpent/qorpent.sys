using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Единица обновления БД
	/// </summary>
	public class DataDiffItem{
		/// <summary>
		/// 
		/// </summary>
		public DataDiffItem(){
			Fields = new ConcurrentDictionary<string, string>();
		}
		/// <summary>
		/// Ид
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Код
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> Fields { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var sb = new StringBuilder();
			sb.Append("Id: " + Id+"; ");
			sb.Append("Code: " + Code + "; ");
			foreach (var f in Fields.OrderBy(_=>_.Key)){
				if("id"==f.Key.ToLower())continue;
				if("code"==f.Key.ToLower())continue;
				sb.AppendFormat("{0}: {1}; ", f.Key, f.Value);
			}
			return sb.ToString();
		}
	}
}