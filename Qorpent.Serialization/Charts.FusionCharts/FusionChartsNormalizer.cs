using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IСhartNormalizer), Name = "fusion.chart.normalizer")]  
    public class FusionChartNormalizer : ConfigBase, IСhartNormalizer {
        /// <summary>
        ///     Внутренний экземпляр конфига чарта
        /// </summary>
        private IChartConfig _chartConfig;
        /// <summary>
        ///     Произвести нормализацию чарта
        /// </summary>
        /// <param name="chart">Чарт</param>
        /// <returns>Нормализованный чарт</returns>
        public IChart Normalize(IChart chart) {
            FitYAxisHeight(chart);
            FixNumberScaling(chart);

            if (IsMultiserial(chart)) {
                FixZeroAnchors(chart);
                FitLabels(chart);
            }

            return chart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        private void FitLabels(IChart chart) {
            var rel = Convert.ToDouble(_chartConfig.Height)/2;

            var src = new Dictionary<int, IEnumerable<IChartDataItem>>();

            if (!chart.Datasets.Children.Any()) {
                return;
            }

            for (int k = 0; k < chart.Datasets.Children.FirstOrDefault().Children.Count; k++ ) {
                var l = new List<IChartDataItem>();

                foreach (var c in chart.Datasets.Children) {
                    l.Add(c.Children.Skip(k).FirstOrDefault());
                }

                src.Add(k, l);
            }

            foreach (var el in chart.Datasets.Children.SelectMany(_ => _.Children)) {
                el.Set(FusionChartApi.Set_ValuePosition, "bottom");
            }

            foreach (var el in src) {
                var ordered = el.Value.Select(_ => _.Get<double>(FusionChartApi.Set_Value)).OrderBy(_ => _).ToList();
                var soClose = ordered.Skip(1).Where(_ => Math.Abs(_ - ordered.Previous(_)) < rel).ToList();

                foreach (var e in soClose) {
                    var i = el.Value.FirstOrDefault(_ => _.Get<double>(FusionChartApi.Set_Value) == e);
                    if (i == null) continue;
                    i.Set(FusionChartApi.Set_ValuePosition, "above");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        private void FixNumberScaling(IChart chart) {
            if (!_chartConfig.UseDefaultScaling) {
                chart.Set(FusionChartApi.Chart_FormatNumber, 0);
                chart.Set(FusionChartApi.Chart_FormatNumberScale, 0);
            }
        }
        /// <summary>
        ///     Создание инстанции нормалайзера
        /// </summary>
        /// <param name="chartConfig">Конфиг графика</param>
        /// <returns>Настроенный экземпляр <see cref="IСhartNormalizer"/></returns>
        public static IСhartNormalizer Create(IChartConfig chartConfig) {
            return new FusionChartNormalizer().Initialize(chartConfig);
        }
        /// <summary>
        ///     Инициализация нормалайзера
        /// </summary>
        /// <param name="chartConfig">Конфиг графика</param>
        /// <returns>Настроенный экземпляр <see cref="IСhartNormalizer"/></returns>
        public IСhartNormalizer Initialize(IChartConfig chartConfig) {
            _chartConfig = chartConfig;
            return this;
        }
        /// <summary>
        ///     Подгонка начальных значений y-оси
        /// </summary>
        /// <param name="chart">Конфиг графика</param>
        private void FitYAxisHeight(IChart chart) {
            var max = GetMaxValue(chart);
            var min = GetMinValue(chart);

            min = min.RoundDown(min.GetNumberOfDigits() - 1);
            max = max.RoundUp(max.GetNumberOfDigits() - 1);

            chart.SetYAxisMinValue(min);
            chart.SetYAxisMaxValue(max);

            FitYAxisNumDivLines(chart, min, max);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private double GetMinValue(IChart chart) {
            var min = GetMinDataset(chart);

            if (chart.TrendLines.Children.Any()) {
                min = min.Minimal(GetMinTrendline(chart).ToInt(true));
            }

            return min;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private double GetMaxValue(IChart chart) {
            var max = GetMaxDataset(chart);

            if (chart.TrendLines.Children.Any()) {
                max = max.Maximal(GetMaxTrendline(chart).ToInt(true));
            }

            return max;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void FitYAxisNumDivLines(IChart chart, double min, double max) {
            var delta = max - min;
            var deltaDigits = delta.GetNumberOfDigits();
            var numDivLines = 0;
            var height = Convert.ToDouble(_chartConfig.Height);

            if (deltaDigits >= 4) {
                numDivLines = (delta / Math.Pow(10, deltaDigits - 2)).ToInt() - 1;
            } else if (deltaDigits >= 3) {
                numDivLines = (delta / Math.Pow(10, deltaDigits - 1)).ToInt() - 1;
            }

            if (delta >= height*2) {
                var dh = delta/height;
                numDivLines = numDivLines/(dh).RoundUp(dh.GetNumberOfDigits() - 2);
            }

            chart.SetNumDivLines(numDivLines);
        }
        /// <summary>
        ///     Фиксит нулевые значения радиусов и сторон якорей вершин
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        private void FixZeroAnchors(IChart chart) {
            chart.Datasets.Children.Where(
                _ => (
                    (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0)
                        ||
                    (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0)
                )
            ).DoForEach(_ => {
                if (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0) {
                    _.Set(FusionChartApi.Chart_AnchorRadius, 5);
                }

                if (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0) {
                    _.Set(FusionChartApi.Chart_AnchorSides, 3);
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private IEnumerable<double> GetTrendlines(IChart chart) {
            return chart.TrendLines.Children.Select(
                _ => _.Get<double>(ChartDefaults.ChartLineStartValue)
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private double GetMaxTrendline(IChart chart) {
            return GetTrendlines(chart).Max();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private double GetMinTrendline(IChart chart) {
            return GetTrendlines(chart).Min();
        }
        /// <summary>
        ///     Возвращает минимальное значение из всех датасетов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Минимальное значение из всех датасетов</returns>
        private double GetMinDataset(IChart chart) {
            return GetDatasetValues(chart).Min();
        }
        /// <summary>
        ///     Возвращает максимальное значение из всех датасетов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Максимальное значение датасета</returns>
        private double GetMaxDataset(IChart chart) {
            return GetDatasetValues(chart).Max();
        }
        /// <summary>
        ///     Возвращает плоский список всех значений датасетов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление значений датасетов</returns>
        private IEnumerable<double> GetDatasetValues(IChart chart) {
            return chart.Datasets.Children.SelectMany(
                _ => _.Children
            ).Select(
                _ => _.Get<double>(FusionChartApi.Set_Value)
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private bool IsMultiserial(IChart chart) {
            var f = chart.Config.Type.To<FusionChartType>();
            if (
                (f & (FusionChartType)FusionChartGroupedType.MultiSeries) == f
            ) {
                return true;
            }

            return false;
        }
    }
}
