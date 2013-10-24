using System.Linq;
using Qorpent.Charts.FusionCharts;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public static class ChartBuilder {
        /// <summary>
        ///     Парсит датасеты вида 100.2,50.50;23.5,66.4 и выдаёт готовый график
        /// </summary>
        /// <param name="chartData">Представление датасетов</param>
        /// <returns>Сформированный график</returns>
        public static IChart ParseDatasets(string chartData) {
            var chart = new Chart();
            var datasets = chartData.Split(new[] { ';' });
            chart.Config = chart.Config ?? new ChartConfig();

            foreach (var ds in datasets) {
                var dataset = new ChartDataset();

                foreach (var value in ds.Split(new[] { ',' })) {
                    dataset.Add(new ChartSet().SetValue(value.ToDecimal()));
                }

                chart.Add(dataset);
            }

            if (datasets.Length > 1) {
                chart.Config.SetChartType(FusionChartType.MSLine);

                for (var i = 0; i < chart.Datasets.Children.Select(_ => _.Children.Count()).Max(); i++) {
                    chart.Add(new ChartCategory().SetLabelValue(""));
                }
            }

            return chart;
        }
    }
}
