using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Колонка значений графика для расчета их позиций
    /// </summary>
    public class DataItemColon : IEnumerable<DataItem> {
	    /// <summary>
	    ///     Внутренний список <see cref="DataItem"/>
	    /// </summary>
	    private readonly DataItem[] _dataItems;
        /// <summary>
        ///     Массив <see cref="DataItem"/>, формирующих данную колонку в виде <see cref="DataItemColon"/>
        /// </summary>
        public DataItem[] Items {
            get { return _dataItems; }
        }
        /// <summary>
        ///     Колонка значений графика
        /// </summary>
        /// <param name="dataItems">Перечисление <see cref="DataItem"/>, образующих колонку</param>
        public DataItemColon(IEnumerable<DataItem> dataItems) {
	        _dataItems = dataItems.ToArray();
        }
		/// <summary>
		/// 
		/// </summary>
	    public int Length{
		    get { return Items.Length; }
	    }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
	    public DataItem this[int idx]{
		    get { return Items[idx]; }
	    }
        /// <summary>
        ///     Приведение <see cref="DataItemColon"/> к <see cref="string"/>
        /// </summary>
        /// <returns><see cref="DataItemColon"/> в виде <see cref="string"/></returns>
        public override string ToString() {
            return this.Aggregate(string.Empty, (_, __) => _ + "," + __, _ => _.Trim(new[] {','}));
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по <see cref="DataItem"/></returns>
        public IEnumerator<DataItem> GetEnumerator() {
	        return _dataItems.AsEnumerable().GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по <see cref="DataItem"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}