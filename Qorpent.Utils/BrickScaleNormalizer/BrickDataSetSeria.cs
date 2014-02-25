using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        ///     Ряды данных внутри серии
        /// </summary>
        public IEnumerable<DataRow> Rows {
            get { return _rows.AsEnumerable(); }
        }
        /// <summary>
        ///     Представление серии из <see cref="DataRow"/>
        /// </summary>
        /// <param name="seriaNumber">Номер серии</param>
        public BrickDataSetSeria(int seriaNumber) {
            SeriaNumber = seriaNumber;
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
        ///     Убеждается в наличии ряда и создаёт его при необходимости
        /// </summary>
        /// <param name="rownum">Номер ряда</param>
        /// <param name="scaleType">Тип шкалы</param>
        /// <returns>Экземпляр <see cref="DataRow"/>, представляющий ряд</returns>
        public DataRow EnsureRow(int rownum, ScaleType scaleType = ScaleType.First) {
            var row = _rows.FirstOrDefault(_ => _.RowNumber == rownum && _.ScaleType == scaleType);
            lock (_rows) {
                if (row == null) {
                    row = new DataRow {RowNumber = rownum, SeriaNumber = SeriaNumber, ScaleType = scaleType};
                    _rows.Add(row);
                }
            }
            return row;
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