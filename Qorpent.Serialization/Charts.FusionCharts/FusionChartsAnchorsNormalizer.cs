using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Нормалайзер якорей величин графиков
    /// </summary>
    public class FusionChartsAnchorsNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            chart.Datasets.Children.Where(
                _ => (
                         (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0)
                         ||
                         (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0)
                     )
                ).DoForEach(_ => {
                    if (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0) {
                        normalized.AddFixedAttribute(_, FusionChartApi.Chart_AnchorRadius, 5);
                    }

                    if (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0) {
                        normalized.AddFixedAttribute(_, FusionChartApi.Chart_AnchorSides, 3);
                    }
                });

            return normalized;
        }
    }
}