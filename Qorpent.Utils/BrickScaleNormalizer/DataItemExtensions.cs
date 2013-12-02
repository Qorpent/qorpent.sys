using System;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для <see cref="DataItem"/>
    /// </summary>
    internal static class DataItemExtensions {
        /// <summary>
        ///     Определяет признак наличия коллизии двух <see cref="DataItem"/>
        /// </summary>
        /// <param name="baseDataItem">Исходный <see cref="DataItem"/></param>
        /// <param name="dataItem"><see cref="DataItem"/> для сравнения</param>
        /// <returns>Признак наличия коллизии</returns>
        public static bool IsCollision(this DataItem baseDataItem, DataItem dataItem) {
            return baseDataItem.Temperature(dataItem) != 0;
        }
        /// <summary>
        ///     Вычисление температуры двух <see cref="DataItem"/>
        /// </summary>
        /// <param name="baseDataItem">Исходный <see cref="DataItem"/></param>
        /// <param name="dataItem"><see cref="DataItem"/> для сравнения</param>
        /// <returns>Температура</returns>
        public static decimal Temperature(this DataItem baseDataItem, DataItem dataItem) {
            if (
                (baseDataItem.LabelPosition.HasFlag(LabelPosition.Hidden) || dataItem.LabelPosition.HasFlag(LabelPosition.Hidden)) ||
                baseDataItem == dataItem ||
                baseDataItem.NormalizedLabelMax < dataItem.NormalizedLabelMin ||
                baseDataItem.NormalizedLabelMin > dataItem.NormalizedLabelMax
                ) {
                return 0;
            }

            return Math.Abs(DataItem.LabelHeight - Math.Abs(baseDataItem.NormalizedLabelMax - dataItem.NormalizedLabelMax));
        }
        /// <summary>
        ///     Прячет «лычку» у элемента данных графика
        /// </summary>
        /// <param name="dataItem">Представления элемента данных графика</param>
        public static void HideLabel(this DataItem dataItem) {
            dataItem.LabelPosition = LabelPosition.Hidden;
        }
    }
}