using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.AbstractCanvas;
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
            FixFontColors(chart);
            FixNumberScaling(chart);

            if (chart.IsMultiserial()) {
                FixZeroAnchors(chart);
                FixLabelsPosition(chart);
            }

            return chart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        private void FixFontColors(IChart chart) {
            foreach (var dataset in chart.Datasets.Children) {
                if (string.IsNullOrWhiteSpace(dataset.GetColor())) {
                    continue;
                }

                if (Convert.ToInt32(dataset.GetColor(), 16)/20 > 1) {
                    dataset.SetColor("FF0000");
                }
            }
        }
        private void FixLabelsPosition(IChart chart) {
            if (!chart.Datasets.Children.Any()) {
                return;
            }

            var src = new List<IList<IChartDataItem>>();
            var dst = new List<IList<ICanvasPrimitive>>();
            var canvas = new Canvas(0, chart.Config.Width.ToInt(), chart.GetYAxisMinValue(), chart.GetYAxisMaxValue());
            canvas.Scale(chart.Config.Width.ToInt(), chart.Config.Height.ToInt());
            for (var k = 0; k < chart.GetLabelCount(); k++) {
                src.Add(chart.Datasets.Children.Select(
                    _ => _.Children.Skip(k).FirstOrDefault()).ToList()
                );
            }

            SetBottomPositionForLabels(chart);

            src.DoForEach(_ => {
                var xpos = ((canvas.X.EndValue - canvas.X.BeginValue) / src.Count) * src.IndexOf(_);
                var nb = new List<ICanvasPrimitive>();
                _.DoForEach(__ => {
                    var p = canvas.SetPoint(xpos, Convert.ToDouble(__.GetValue())).SetOwner(__);
                    nb.Add(p);
                });
                dst.Add(nb);
            });

            dst.DoForEach(
                _ => _.DoForEach(
                    __ => _.SelectMany(
                        ___ => canvas.Nearby(___, 20)
                    ).Skip(1).DoForEach(
                        ____ => {
                            if (canvas.Nearby(____, 5).Any()) {
                                (____.Owner as IChartDataItem).SetShowValue(false);
                            } else {
                                var nb15 = canvas.Nearby(____, 15).ToList();

                                if (!nb15.Any()) {
                                    return;
                                }
                                var highest = nb15.FirstOrDefault(
                                    _____ => ____.X == nb15.OrderByDescending(______ => ______.X).First().X
                                );

                                if (highest.X > ____.X) {
                                    var owner = highest as IChartDataItem;

                                    if (owner != null) {
                                        owner.Set(FusionChartApi.Chart_LabelPosition, "above");
                                    }
                                } else {
                                    (____.Owner as IChartDataItem).SetShowValue(false);
                                }
                            }
                        }
                    )
                )
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        private void SetBottomPositionForLabels(IChart chart) {
            foreach (var el in chart.Datasets.Children.SelectMany(_ => _.Children)) {
                el.Set(FusionChartApi.Set_ValuePosition, "bottom");
                el.SetShowValue(true);
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
            var max = chart.GetYMaxValueWholeChart();
            var min = chart.GetYMinValueWholeChart();

            if ((max != 0.0) && (min != 0.0)) {
                min = min.RoundDown(min.GetNumberOfDigits() - 1);
                max = max.RoundUp(max.GetNumberOfDigits() - 1);
                chart.SetYAxisMinValue(min);
                chart.SetYAxisMaxValue(max);

                FitYAxisNumDivLines(chart, min, max);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void FitYAxisNumDivLines(IChart chart, double min, double max) {
            var deltaDigits = chart.GetDelta().GetNumberOfDigits();
            var numDivLines = 0;
            var height = Convert.ToDouble(_chartConfig.Height);

            if (deltaDigits >= 4) {
                numDivLines = (chart.GetDelta() / Math.Pow(10, deltaDigits - 2)).ToInt() - 1;
            } else if (deltaDigits >= 3) {
                numDivLines = (chart.GetDelta() / Math.Pow(10, deltaDigits - 1)).ToInt() - 1;
            }

            if (chart.GetDelta() >= height * 2) {
                var dh = chart.GetDelta() / height;
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
    }
}
