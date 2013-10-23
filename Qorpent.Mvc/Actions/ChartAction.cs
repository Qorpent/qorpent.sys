using System;
using System.Linq;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
    /// <summary>
    /// 
    /// </summary>
    [Action("_sys.chart", Role = "DEFAULT")]
    public class ChartAction : ActionBase {
        /// <summary>
        ///     Данные для чарта
        /// </summary>
        [Bind(Required = true)]
        public string ChartData { get; set; }
        /// <summary>
        /// Основная фаза - тело действия
        /// </summary>
        /// <returns> </returns>
        protected override object MainProcess() {
            return GenerateCustomSetChart(ChartData);
        }
        /// <summary>
        ///     Собирает примитивный график по переданному массиву данных
        /// </summary>
        /// <param name="chartData">Массив данных согласно #AP-311</param>
        /// <returns>Представление графика</returns>
        public static IChart GenerateCustomSetChart(string chartData) {
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

                for (int i = 0; i < chart.Datasets.Children.Select(_ => _.Children.Count()).Max(); i++) {
                    chart.Add(new ChartCategory().SetLabelValue(""));
                }
            }

            return chart;
        }
    }
}
