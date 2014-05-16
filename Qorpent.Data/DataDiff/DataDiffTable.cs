using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.DataDiff{

	

	/// <summary>
	/// 
	/// </summary>
	public class DataDiffTable
	{
		/// <summary>
		/// 
		/// </summary>
		public DataDiffTable(){
			Definitions = new List<DataDiffItem>();
			Sources = new List<XDiffItem>();
			Mappings = new Dictionary<string, string>();
		}
		/// <summary>
		/// Имя целевой табицы
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// Итоговые определения
		/// </summary>
		public IList<DataDiffItem> Definitions { get; private set; }
		/// <summary>
		/// Исходные дельты
		/// </summary>
		public IList<XDiffItem> Sources { get; private set; }
		/// <summary>
		/// Мэпинг внешних ключей между таблицами
		/// </summary>
		public IDictionary<string, string> Mappings { get; private set; }
		/// <summary>
		/// Признак использования для связки истории кодов
		/// </summary>
		public bool UseAliasCodes { get; set; }
		/// <summary>
		/// Признак того, что все обновления должны включать в себя код проекта и ревизию в проекте при обновлении
		/// </summary>
		public bool UseRevisions { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var sb = new StringBuilder();
			sb.AppendLine("Update table : " + TableName);
			sb.AppendLine("Sources : " + Sources.Count);
			sb.AppendLine("Definitions : " + Definitions.Count);
			foreach (var dataDiffItem in Definitions.OrderBy(_=>_.Id).ThenBy(_=>_.Code)){
				sb.AppendLine("\t"+dataDiffItem);
			}
			return sb.ToString();
		}
	}
}