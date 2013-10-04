using System;
using System.Collections.Generic;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Нормалайзер
    /// </summary>
    public static class FusionChartsNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public const int MinValuePadding = 10;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IChart Normalize(this IChart chart, FusionChartsNormalizerConfig config) {
            if (chart is Chart) {
                return NormalizeChart(chart as Chart, config);
            }

            throw new NotSupportedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static Chart NormalizeChart(Chart chart, FusionChartsNormalizerConfig config) {
            foreach (var dataset in chart.Datasets.AsList) {
                foreach (var ds in dataset.AsList) {
                    foreach (var c in ds.Children) {
                        Console.WriteLine(c.Count);
                    }
                }

            }

            return chart;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public struct FusionChartsNormalizerConfig {
        /// <summary>
        ///    Установленный параметер указывает на то, что нормалайзер должен сделать «разрыв» графика 
        /// </summary>
        public bool ReduceY;
    }
}
