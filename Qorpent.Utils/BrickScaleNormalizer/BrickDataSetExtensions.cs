using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для <see cref="BrickDataSet"/>
    /// </summary>
    public static class BrickDataSetExtensions {
        /// <summary>
        ///     Возвращает перечисление <see cref="DataRow"/> внутри <see cref="BrickDataSet"/> по номеру серии
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <param name="serianum">Номер серии</param>
        /// <returns>Перечисление <see cref="DataRow"/></returns>
        public static IEnumerable<DataRow> GetSeriaRows(this BrickDataSet dataSet, int serianum) {
            return dataSet.Rows.Where(_ => _.SeriaNumber == serianum);
        }
        /// <summary>
        ///     Возвращает упорядоченное перечисление серий из <see cref="BrickDataSet"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление серий из <see cref="BrickDataSet"/></returns>
        public static IEnumerable<BrickDataSetSeria> GetSeries(this BrickDataSet dataSet) {
            return dataSet.Rows.GroupBy(_ => _.SeriaNumber).Select(_ => new BrickDataSetSeria(_.Key, _.Select(__ => __)));
        }
        /// <summary>
        ///     Собирает упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/></returns>
        public static IEnumerable<DataItemColon> GetColons(this BrickDataSet dataSet) {
            return dataSet.Rows.SelectMany(_ => _.Items).GroupBy(_ => _.Index).Select(_ => new DataItemColon(_.Select(__ => __)));
        }
    }
}