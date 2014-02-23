using System;
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
        ///     Добавление элемента данных в ряд
        /// </summary>
        /// <param name="dataItem">Элемент данных</param>
        public void Add(DataItem dataItem) {
            dataItem.Index = Items.Count;
            Insert(dataItem);
        }
        /// <summary>
        ///     Прямая вставка элемента данных в ряд
        /// </summary>
        /// <param name="dataItem">Элемент данных</param>
        public void Insert(DataItem dataItem) {
            lock (Items) {
                if (dataItem.Index < Items.Count) {
                    throw new Exception("Incorrect index number");
                }
                Items.Add(dataItem);
            }
        }
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