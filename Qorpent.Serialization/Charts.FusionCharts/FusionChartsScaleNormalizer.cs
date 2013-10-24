using System;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            normalized.AddScale(NormalizeYAxis(chart));
            return normalized;
        }
        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private ChartAbstractScale NormalizeYAxis(IChart chart) {
            var abstractScale = new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = 0,
                MaxValue = 0.0,
                MinValue = 0.0
            };

            if (!chart.Datasets.Children.Any()) {
                return abstractScale;
            }

            var min = chart.GetYMinValueWholeChart();
            var max = chart.GetYMaxValueWholeChart();

            var normalizedMin = NormalizeMinValue(min);
            var normalizedMax = NormalizeMaxValue(max);

            while (normalizedMin >= min) {
                normalizedMin = NormalizeMinValue(normalizedMin - Math.Pow(10, normalizedMin.GetNumberOfDigits() - 1));
            }

            while (normalizedMax <= max) {
                normalizedMax = NormalizeMaxValue(normalizedMax + Math.Pow(10, normalizedMax.GetNumberOfDigits() - 1));
            }

            abstractScale.MinValue = normalizedMin;
            abstractScale.MaxValue = normalizedMax;
            abstractScale.NumDivLines = NormalizeNumDivlines(abstractScale);

            return abstractScale;
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
