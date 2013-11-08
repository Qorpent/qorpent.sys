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

            ResolveDivlines(chart, normalizedScale);

            return new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = normalizedScale.RecommendedVariant.Divline,
                MaxValue = normalizedScale.RecommendedVariant.Maximal,
                MinValue = normalizedScale.RecommendedVariant.Minimal
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="normalizedScale"></param>
        private void ResolveDivlines(IChart chart, ScaleNormalized normalizedScale) {
            var scale = normalizedScale.RecommendedVariant;

            if (chart.Config.Height.ToInt() > 600) {
                var pixels = chart.Config.Height.ToDecimal()*0.8m;
                if (scale.Divline <= 3) {
                    scale.DoubleDivlines();
                }

                var shrinked = (scale.Maximal - scale.DivSize).ToDecimal();
                var pixnorm = ((scale.Maximal - scale.Minimal).ToDecimal()/pixels);

                if (((shrinked - scale.Minimal.ToDecimal())*pixnorm) >=
                    ((normalizedScale.Approximated.BaseMaximal - scale.Minimal).ToDecimal()*pixnorm + 40)) {
                    scale.Maximal = Convert.ToDouble(shrinked);
                    scale.Divline--;
                }
            } else {
                var oldDivline = scale.Divline;
                var divided = (scale.Divline/2).ToDouble();
                var delta = scale.Maximal - scale.Minimal;
                scale.Divline = Math.Floor(divided).ToInt();
                if ((delta % scale.DivSize).ToInt() != 0) {
                    scale.Divline = oldDivline;
                }
                if (scale.Divline == 0) {
                    scale.Divline = 1;
                }
            }
        }
    }
}