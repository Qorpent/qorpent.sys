namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public static class ChartExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IChart AddElement(this IChart chart, IChartElement element) {
            if (element is IChartDataset) {
                chart.Datasets.Add(element as IChartDataset);
            } else if (element is IChartCategory) {
                chart.Categories.Add(element as IChartCategory);
            } else if (element is IChartDataItem) {
                var p = ParentOf<IChartDataset>(element);
                
                if (p != null) {
                    p.Children.Add(element as IChartDataItem);
                }
            } else if (element is IChartTrendLine) {
                chart.TrendLines.Add(element as IChartTrendLine);
            } else if (element is IChartLineSet) {

            }

            return chart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static T ParentOf<T>(IChartElement element) {
            return element.Get<T>(ChartDefaults.ChartElementParentProperty);
        }
    }
}
