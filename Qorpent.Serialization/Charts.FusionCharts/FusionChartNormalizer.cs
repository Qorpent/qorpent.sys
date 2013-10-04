using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     
    /// </summary>
    public class FusionChartNormalizer : ConfigBase {
        /// <summary>
        ///     Внутренний экземпляр конфига чарта
        /// </summary>
        private IChartConfig _chartConfig;
        /// <summary>
        ///     Величина, на которую нужно вывести паддинги от максимального и минимального
        ///     значений графика относительно верхней и нижней границы
        /// </summary>
        public double XAxisMargin {
            get { Get<double>("XAxisMargin", 100); }
            set { Set("XAxisMargin", value); }
        }
        /// <summary>
        ///     Произвести нормализацию чарта
        /// </summary>
        /// <param name="chart">Чарт</param>
        /// <returns>Нормализованный чарт</returns>
        public IChart Normalize(IChart chart) {
            FitYAxisHeight(chart);
            return chart;
        }
        /// <summary>
        ///     Создание инстанции нормалайзера
        /// </summary>
        /// <param name="chartConfig">Конфиг графика</param>
        /// <returns>Настроенный экземпляр <see cref="FusionChartNormalizer"/></returns>
        public static FusionChartNormalizer Create(IChartConfig chartConfig) {
            return new FusionChartNormalizer().Initialize(chartConfig);
        }
        /// <summary>
        ///     Инициализация нормалайзера
        /// </summary>
        /// <param name="chartConfig">Конфиг графика</param>
        /// <returns>Настроенный экземпляр <see cref="FusionChartNormalizer"/></returns>
        public FusionChartNormalizer Initialize(IChartConfig chartConfig) {
            _chartConfig = chartConfig;
            return this;
        }
        /// <summary>
        ///     Подгонка начальных значений y-оси
        /// </summary>
        /// <param name="chart">Конфиг графика</param>
        private void FitYAxisHeight(IChart chart) {
            chart.Set("yAxisMinValue", Math.Round(GetMinDataset(chart).ToInt() / 100.0) * 100 - XAxisMargin);
            chart.Set("yAxisMaxValue", Math.Round(GetMaxDataset(chart).ToInt() / 100.0) * 100 + XAxisMargin);
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
    }
}
