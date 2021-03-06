﻿using System.Linq;
using Qorpent.Charts.FusionCharts;
using Qorpent.Utils.BrickScaleNormalizer;
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
            var chart = new Chart {
                Config = new ChartConfig()
            };

            var datasets = chartData.SmartSplit(false, true, new[] {';'});

            foreach (var ds in datasets) {
                var dataset = new ChartDataset();

                EnumerableExtensions.DoForEach(ds.SmartSplit(false, true, new[] { ',' }), _ => dataset.Add(new ChartSet().SetValue(_.ToDecimal()))
                );


                chart.Add(dataset);
            }

            for (var i = 0; i < chart.Datasets.Children.Select(_ => _.Children.Count()).Max(); i++) {
                chart.Add(new ChartCategory().SetLabelValue(""));
            }

            return chart;
        }
        /// <summary>
        ///     Преобразует данные из <see cref="BrickDataSet"/> в <see cref="IChart"/>
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет в виде <see cref="BrickDataSet"/></param>
        /// <returns>Эквивалентный экземпляр <see cref="IChart"/></returns>
        public static IChart ParseBrickDataSet(BrickDataSet brickDataSet) {
            return ParseDatasets(string.Join(";", brickDataSet.GetSeries().Select(_ => _.ToString())));
        }
    }
}
