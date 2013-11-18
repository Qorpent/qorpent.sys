using System.Collections.Generic;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Ряд данных
	/// </summary>
	public class DataRow {
		/// <summary>
		/// 
		/// </summary>
		public DataRow() {
			Items = new List<DataItem>();
		}
		/// <summary>
		/// Шкала
		/// </summary>
		public ScaleType ScaleType { get; set; }
		/// <summary>
		/// Номер ряда
		/// </summary>
		public int RowNumber { get; set; }
		/// <summary>
		/// Номер серии
		/// </summary>
		public int SeriaNumber { get; set; }
		/// <summary>
		/// Значения в серии
		/// </summary>
		public IList<DataItem> Items { get; private set; } 
	}
}