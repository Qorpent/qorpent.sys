using System;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsValuesNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsValuesNormalizer() {
            Code = FusionChartsNormalizerCodes.FusionChartsValuesNormalizer;
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            var delimiter = Convert.ToDecimal(GetDelimiter(chart));

            NormalizeDatasets(chart, delimiter, normalized);
            NormalizeTrendlines(chart, delimiter, normalized);

            return normalized;
        }
        private void NormalizeTrendlines(IChart chart, decimal delimiter, IChartNormalized normalized) {
            chart.GetTrendlines().DoForEach(
                _ => normalized.AddFixedAttribute(_, ChartDefaults.ChartLineStartValue, _.GetStartValue() / Convert.ToDouble(delimiter))
            );
        }
        /// <summary>
        ///     Нормализация датасетов
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="delimiter"></param>
        /// <param name="normalized"></param>
        private void NormalizeDatasets(IChart chart, decimal delimiter, IChartNormalized normalized) {
            chart.Datasets.Children.SelectMany(
                _ => _.Children
            ).DoForEach(
                _ => normalized.AddFixedAttribute(_, FusionChartApi.Set_Value, Math.Round(_.GetValue() / delimiter))
            );
        }
        /// <summary>
        ///     Получение делителя значений для приведения
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Делитель</returns>
        private double GetDelimiter(IChart chart) {
            switch (chart.EnsureConfig().ShowValuesAs) {
                case ChartShowValuesAs.AsIs: return 1.0;
                case ChartShowValuesAs.Tens: return 10.0;
                case ChartShowValuesAs.Hundreds: return 100.0;
                case ChartShowValuesAs.Thousands: return 1000.0;
                case ChartShowValuesAs.TensOfThousands: return 10000.0;
                case ChartShowValuesAs.HundredsOfThousands: return 100000.0;
                case ChartShowValuesAs.Millions: return 1000000.0;
                default: return 1.0;
            }
        }
    }
}
