using System;
using Qorpent.Utils;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Нормалайзер цветов чарта
    /// </summary>
    public class FusionChartsColorNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            foreach (var dataset in chart.Datasets.Children) {
                if (string.IsNullOrWhiteSpace(dataset.GetColor())) {
                    continue;
                }

                if (Convert.ToInt32(dataset.GetColor(), 16) / 20 > 1) {
                    normalized.AddFixedAttribute(dataset, FusionChartApi.Dataset_Color, new Hex("FF0000"));
                }
            }

            return normalized;
        }
    }
}