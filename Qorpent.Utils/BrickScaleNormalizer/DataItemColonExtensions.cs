using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для <see cref="DataItemColon"/>
    /// </summary>
    internal static class DataItemColonExtensions {
        /// <summary>
        ///     Выбирает одинаковые значения в колонке
        /// </summary>
        /// <param name="colon">Представление колонки датасета</param>
        /// <returns>Перечисление групп значений, которые совпадают между собой</returns>
        public static IEnumerable<IGrouping<decimal, DataItem>> SelectSimilar(this DataItemColon colon) {
            return colon.GroupBy(_ => _.Value).Where(_ => _.Count() > 1);
        }
    }
}