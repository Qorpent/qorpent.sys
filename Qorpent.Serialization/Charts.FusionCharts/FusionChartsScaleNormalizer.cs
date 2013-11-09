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

            var scale = normalizedScale.RecommendedVariant;

            if (scale.Minimal < 0 && scale.Maximal > 0) {
                if (scale.Maximal.Minimal(scale.Minimal.Abs())/scale.Maximal.Maximal(scale.Minimal.Abs()).Abs() >= 0.8) {
                    var absMax = scale.Maximal.Maximal(scale.Minimal.Abs()).Abs();
                    scale.Minimal = absMax*(-1);
                    scale.Maximal = absMax;
                }
            }

            ResolveDivlines(chart, normalizedScale);
            ResolveMinimals(chart, normalizedScale);
            ResolveMaximals(chart, normalizedScale);

            return new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = normalizedScale.RecommendedVariant.Divline,
                MaxValue = normalizedScale.RecommendedVariant.Maximal,
                MinValue = normalizedScale.RecommendedVariant.Minimal
            };
        }

        /// <summary>
        ///     Увеличивает отступ от минимального значения вниз, если это требуется
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        private void ResolveMinimals(IChart chart, ScaleNormalized normalized) {
            while (true) {
                var currentPadding = GetPixelApproximation(chart, normalized, normalized.RecommendedVariant.Minimal, normalized.Approximated.BaseMinimal).Abs();
                if (!(currentPadding < 20)) {
                    break;
                }

                normalized.RecommendedVariant.Minimal -= normalized.RecommendedVariant.DivSize;
                normalized.RecommendedVariant.Divline++;
            }
        }
        /// <summary>
        ///     Увеличивает отступ от максимального значения вверх, если это требуется
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        private void ResolveMaximals(IChart chart, ScaleNormalized normalized) {
            while (true) {
                var currentPadding = GetPixelApproximation(chart, normalized, normalized.RecommendedVariant.Maximal, normalized.Approximated.BaseMaximal);
                if (!(currentPadding < 20)) {
                    break;
                }

                normalized.RecommendedVariant.Maximal += normalized.RecommendedVariant.DivSize;
                normalized.RecommendedVariant.Divline++;
            }
        }
        /// <summary>
        ///     Рассчитывает текущий отступ в пикселях реального максимального значения от граничного
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        /// <param name="borderMax">Граничное максимальное значение</param>
        /// <param name="realMax">Реальное максимальное значение</param>
        /// <returns>Текущий отступ в пикселях реального максимального значения от граничного</returns>
        private double GetPixelApproximation(IChart chart, ScaleNormalized normalized, double borderMax, double realMax) {
            return (borderMax - realMax) / GetPixnorm(chart, normalized);
        }
        /// <summary>
        ///     Вычисляет приблизительную величину на пиксель
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="normalized">Представление нормализованного графика</param>
        /// <returns>Приблизительная величина на пиксель</returns>
        private double GetPixnorm(IChart chart, ScaleNormalized normalized) {
            return ((normalized.RecommendedVariant.Maximal - normalized.RecommendedVariant.Minimal) / chart.Config.Height.ToInt()).Abs();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="normalized"></param>
        private void ResolveDivlines(IChart chart, ScaleNormalized normalized) {
            var scale = normalized.RecommendedVariant;
            if (scale.Divline <= 3) {
                scale.DoubleDivlines();
            }

            if (chart.Config.Height.ToInt() > 600) {
                while (true) {
                    var shrinked = (scale.Maximal - scale.DivSize);
                    var newMaximal = GetPixelApproximation(chart, normalized, shrinked, normalized.Approximated.BaseMaximal);

                    if (newMaximal > 20) {
                        scale.Maximal = shrinked;
                        scale.Divline--;
                    } else {
                        break;
                    }
                }
            } else {
                if (normalized.RecommendedVariant.Minimal < 0) {
                    return;
                }
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