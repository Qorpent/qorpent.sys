using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	///     Ряд данных
	/// </summary>
    public class DataRow : Tagged, IEnumerable<DataItem> {
		/// <summary>
		///		Шкала
		/// </summary>
		public ScaleType ScaleType { get; set; }
		/// <summary>
		///		Номер ряда
		/// </summary>
		public int RowNumber { get; set; }
		/// <summary>
		///		Номер серии
		/// </summary>
		public int SeriaNumber { get; set; }
		/// <summary>
		/// Значения в серии
		/// </summary>
		public IList<DataItem> Items { get; private set; }
		/// <summary>
		///     Ряд данных
		/// </summary>
		public DataRow() {
			Items = new List<DataItem>();
		}
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
        ///     Приведение <see cref="DataRow"/> к <see cref="string"/>
        /// </summary>
        /// <returns><see cref="DataRow"/> в виде <see cref="string"/></returns>
        public override string ToString() {
            return Items.Aggregate(string.Empty, (_, __) => _ + "," + __, _ => _.Trim(new[] { ',' }));
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