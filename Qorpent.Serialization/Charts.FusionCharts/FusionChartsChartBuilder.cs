namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsChartBuilder : ChartBuilder {
        /// <summary>
        ///     Собираемый чарт с ленивой инициализацией
        /// </summary>
        protected override IChart Chart {
            get { return BuildChart ?? (BuildChart = new FusionChartChart()); }
        }
    }
}
