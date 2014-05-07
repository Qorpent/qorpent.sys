using System.Collections.Generic;
using System.IO;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Контекст выполнения генерации XDiffTable
	/// </summary>
	public class TableDiffGeneratorContext{
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
	}
}