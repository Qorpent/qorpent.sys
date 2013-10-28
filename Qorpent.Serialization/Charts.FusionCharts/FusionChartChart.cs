
namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     
    /// </summary>
    public class FusionChartChart : Chart {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="chartConfig"></param>
        /// <returns></returns>
        protected override IChartRender CreateChartRender(IChartConfig chartConfig) {
            return new FusionChartRender();
        }
    }
}
