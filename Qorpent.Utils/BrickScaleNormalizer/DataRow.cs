using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	///     Ряд данных
	/// </summary>
    public class DataRow : IEnumerable<DataItem> {
		/// <summary>
        ///     Ряд данных
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
        /// <summary>
        ///     Получение <see cref="IEnumerable"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerable"/> по <see cref="DataItem"/></returns>
        public IEnumerator<DataItem> GetEnumerator() {
            return Items.GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerable"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerable"/> по <see cref="DataItem"/></returns>
	    IEnumerator IEnumerable.GetEnumerator() {
	        return GetEnumerator();
	    }
	}
}