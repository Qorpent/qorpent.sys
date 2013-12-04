using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsPositionNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsPositionNormalizer() {
            Area = ChartNormalizerArea.Labels;
            AddDependency(FusionChartsNormalizerCodes.ScaleNormalizer);
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            if (chart.IsMultiserial()) {
                NormalizeMiltiserial(chart, normalized);
            }

            return normalized;
        }
        /// <summary>
        ///     Нормализует мультисерийный график
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="normalized">Нормализованное представление графика</param>
        private void NormalizeMiltiserial(IChart chart, IChartNormalized normalized) {
            var ds = chart.ToBrickDataset();
            ds.Calculate();
            var chartGenerated = ds.ToChart();
            chart.AddElements(new [] {chartGenerated.Datasets});

        }
    }
}