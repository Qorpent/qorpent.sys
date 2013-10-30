using System;
using System.Linq;
using Qorpent.Utils.Extensions;

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
            chart.EnsureConfig();
            var abstractScale = new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = 0,
                MaxValue = 0.0,
                MinValue = 0.0
            };

            if (!string.IsNullOrWhiteSpace(chart.Config.MinValue) && !string.IsNullOrWhiteSpace(chart.Config.MaxValue)) {
                abstractScale.MinValue = Convert.ToDouble(chart.Config.MinValue);
                abstractScale.MaxValue = Convert.ToDouble(chart.Config.MaxValue);
            } else {
                NormalizeLeftY(normalized, abstractScale);
            }

            abstractScale.NumDivLines = NormalizeNumDivlines(abstractScale);

            return abstractScale;
        }
        private void NormalizeLeftY(IChartNormalized normalized, ChartAbstractScale abstractScale) {
            var min = Convert.ToDouble(normalized.GetFixedAttributes<IChartDataItem, decimal>(FusionChartApi.Set_Value).Min());
            var max = Convert.ToDouble(normalized.GetFixedAttributes<IChartDataItem, decimal>(FusionChartApi.Set_Value).Max());

            var normalizedMax = NormalizeMaxValue(max);
            var normalizedMin = NormalizeMinValue(min);

            if ((min >= 0) && (max >= 0)) {
                if (min < 100 && max > 100) {
                    normalizedMin = 0;
                }

                while (normalizedMax <= max) {
                    normalizedMax = NormalizeMaxValue(normalizedMax + Math.Pow(10, normalizedMax.GetNumberOfDigits() - 1));
                }

                while (normalizedMin >= min) {
                    normalizedMin = NormalizeMinValue(normalizedMin - Math.Pow(10, normalizedMin.GetNumberOfDigits() - 1));
                }
            }

            abstractScale.MinValue = normalizedMin;
            abstractScale.MaxValue = normalizedMax;
        }
        /// <summary>
        ///     Нормализует дивлайны
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        /// <returns></returns>
        private int NormalizeNumDivlines(ChartAbstractScale abstractScale) {
            var delta = abstractScale.MaxValue - abstractScale.MinValue;
            return ((delta/Math.Pow(10, delta.GetNumberOfDigits() - 1)) - 1).ToInt();
        }
        /// <summary>
        ///     Нормализует минимальное значение
        /// </summary>
        /// <param name="num">Число для округления</param>
        /// <returns></returns>
        private double NormalizeMinValue(double num) {
            return !num.IsRoundNumber(num.GetNumberOfDigits() - 1) ? num.RoundDown(num.GetNumberOfDigits() - 1) : num;
        }
        /// <summary>
        ///     Нормализует максимальное значение.
        /// </summary>
        /// <remarks>
        ///     Если число круглое — возвращает его же, иначе — округляет вверх по порядку N - 1, где N - порядок переданного числа
        ///     Примеры: 500 -> 500, 510 -> 600, 599 -> 600, 600 -> 600, 577.5 -> 600, 500.1 -> 600
        /// </remarks>
        /// <param name="num">Число для округления</param>
        /// <returns></returns>
        private double NormalizeMaxValue(double num) {
            return !num.IsRoundNumber(num.GetNumberOfDigits() - 1) ? num.RoundUp(num.GetNumberOfDigits() - 1) : num;
        }
    }
}
