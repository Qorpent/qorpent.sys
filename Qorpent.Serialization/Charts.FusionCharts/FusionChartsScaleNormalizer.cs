using System;
using System.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsScaleNormalizer() {
            Code = FusionChartsNormalizerCodes.ScaleNormalizer;
            Area = ChartNormalizerArea.YScale;
            AddDependency(FusionChartsNormalizerCodes.FusionChartsValuesNormalizer);
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            normalized.AddScale(NormalizeYAxis(chart, normalized));
            return normalized;
        }

        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private ChartAbstractScale NormalizeYAxis(IChart chart, IChartNormalized normalized) {
            var normalizedScale = ScaleNormalizer.Normalize(
                new ScaleNormalizeClause(),
                normalized.GetFixedAttributes<decimal>(FusionChartApi.Set_Value).Select(Convert.ToDouble)
            );
            return new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = normalizedScale.RecommendedVariant.Divline,
                MaxValue = normalizedScale.RecommendedVariant.Minimal,
                MinValue = normalizedScale.RecommendedVariant.Maximal
            };
        }
    }
}
