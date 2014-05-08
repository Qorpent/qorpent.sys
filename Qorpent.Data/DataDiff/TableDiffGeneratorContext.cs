using System.Collections.Generic;
using System.IO;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Контекст выполнения генерации XDiffTable
	/// </summary>
	public class TableDiffGeneratorContext{
		/// <summary>
		/// 
		/// </summary>
		public TableDiffGeneratorContext(){
			InTransaction = true;
			IgnoreFields = new List<string>();
		}
		/// <summary>
		/// Подготовленные пары XML для сравнения
		/// </summary>
		public IEnumerable<DiffPair> DiffPairs { get; set; } 
		/// <summary>
		/// Таблицы с измененными данными
		/// </summary>
		public IEnumerable<DataDiffTable> Tables { get; set; }
		/// <summary>
		/// Исходящий поток
		/// </summary>
		public TextWriter SqlOutput { get; set; }

		/// <summary>
		/// Признак необходимости использования транзакций
		/// </summary>
		public bool InTransaction { get; set; }
		/// <summary>
		/// Поля, которые игнорируются при формировании дифа
		/// </summary>
		public IList<string> IgnoreFields { get; private set; }
	}
}