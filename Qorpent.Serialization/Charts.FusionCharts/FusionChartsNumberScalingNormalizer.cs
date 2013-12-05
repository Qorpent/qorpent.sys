namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsNumberScalingNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsNumberScalingNormalizer() {
            Area = ChartNormalizerArea.Formatting;
            Code = FusionChartsNormalizerCodes.ScalingNormalizer;
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            if (chart.Config != null) {
                if (!chart.Config.UseDefaultScaling) {
                    chart.SetDefaultNumberScaling();
                }
            } else {
                chart.SetDefaultNumberScaling();
            }

            return normalized;
        }
    }
}