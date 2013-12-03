using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Представление серии из <see cref="DataRow"/>
    /// </summary>
    public class BrickDataSetSeria : IEnumerable<DataItem> {
        /// <summary>
        ///     Внутренний список <see cref="DataRow"/>, присущих данной серии
        /// </summary>
        private readonly List<DataRow> _rows = new List<DataRow>();
        /// <summary>
        ///     Номер серии
        /// </summary>
        public int SeriaNumber { get; private set; }
        /// <summary>
        ///     Представление серии из <see cref="DataRow"/>
        /// </summary>
        /// <param name="seriaNumber">Номер серии</param>
        public BrickDataSetSeria(int seriaNumber) {
            SeriaNumber = seriaNumber;
        }
        /// <summary>
        ///     Представление серии из <see cref="DataRow"/>
        /// </summary>
        /// <param name="seriaNumber">Номер серии</param>
        /// <param name="rows">Перечисление <see cref="DataRow"/>, присущих данной серии</param>
        public BrickDataSetSeria(int seriaNumber, IEnumerable<DataRow> rows) {
            SeriaNumber = seriaNumber;
            rows.ForEach(Add);
        }
        /// <summary>
        ///     Добавление <see cref="DataRow"/> в серию
        /// </summary>
        /// <param name="dataRow">Экземпляр <see cref="DataRow"/>, связанный с данной серие</param>
        public void Add(DataRow dataRow) {
            if (dataRow.SeriaNumber != SeriaNumber) {
                throw new Exception("Cannot assign a datarow from another seria");
            }

            _rows.Add(dataRow);
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator{T}"/> по <see cref="DataRow"/>
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> по <see cref="DataRow"/></returns>
        public IEnumerator<DataItem> GetEnumerator() {
            return _rows.SelectMany(_ => _.Items).GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator{T}"/> по <see cref="DataRow"/>
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> по <see cref="DataRow"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        /// <summary>
        ///     Приведение серии к строке вида 40.1,50,0.2,15
        /// </summary>
        /// <returns>Строка, представляющая <see cref="BrickDataSetSeria"/></returns>
        public override string ToString() {
            return string.Join(",", this.Select(_ => _.Value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}