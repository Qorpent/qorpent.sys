﻿using System;
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
            
            if (IsMultiserial(chart)) {
                FixZeroAnchors(chart);
            }

            return chart;
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
            var max = GetMaxDataset(chart);
            var min = GetMinDataset(chart);

            if (!_chartConfig.UseDefaultScaling) {
                chart.Set(FusionChartApi.Chart_FormatNumber, 0);
                chart.Set(FusionChartApi.Chart_FormatNumberScale, 0);
            }

            min = min.RoundDown(min.GetNumberOfDigits() - 1);
            max = max.RoundUp(max.GetNumberOfDigits() - 1);

            if (chart.TrendLines.Children.Any()) {
                min = min.Minimal(GetMinTrendline(chart).ToInt(true));
                max = max.Maximal(GetMaxTrendline(chart).ToInt(true));
            }

            min = min.RoundDown(min.GetNumberOfDigits() - 1);
            max = max.RoundUp(max.GetNumberOfDigits() - 1);

            chart.SetYAxisMinValue(min);
            chart.SetYAxisMaxValue(max);

            var delta = max - min;
            var deltaDigits = delta.GetNumberOfDigits();
            var numDivLines = 0;

            if (deltaDigits >= 4) {
                numDivLines = (delta / Math.Pow(10, deltaDigits - 2)).ToInt() - 1;
            } else if (deltaDigits >= 3) {
                numDivLines = (delta / Math.Pow(10, deltaDigits - 1)).ToInt() - 1;
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
