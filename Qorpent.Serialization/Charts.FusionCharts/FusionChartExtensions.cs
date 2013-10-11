using System.Collections.Generic;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public static class FusionChartExtensions {
        /// <summary>
        ///     Добавление сета к датасету
        /// </summary>
        /// <param name="dataset">Исходный датасет</param>
        /// <param name="label">Метка</param>
        /// <param name="value">Значение</param>
        /// <returns>Установленный сет</returns>
        public static IChartSet AddSet(this IChartDataset dataset, string label, decimal value) {
            var set = new ChartSet();
            set.Set(FusionChartApi.Set_Label, label);
            set.Set(FusionChartApi.Set_Value, value);
            dataset.Add(set);
            return set;
        }
        /// <summary>
        ///     Признак того, что график мультисерийный
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Признак того, что график мультисерийный</returns>
        public static bool IsMultiserial(this IChart chart) {
            var f = chart.Config.Type.To<FusionChartType>();
            if (
                (f & (FusionChartType)FusionChartGroupedType.MultiSeries) == f
            ) {
                return true;
            }

            return false;
        }
        /// <summary>
        ///     Возвращает перечисление категорий, относящихся к графику
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление категорий</returns>
        public static IEnumerable<IChartCategory> GetCategories(this IChart chart) {
            return chart.Categories.Children;
        }
        /// <summary>
        ///     Возвращает перечисление датасетов, относящихся к графику
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление датасетов</returns>
        public static IEnumerable<IChartDataset> GetDatasets(this IChart chart) {
            return chart.Datasets.Children;
        }
        /// <summary>
        ///     Возвращает перечисление линий тренда, относящихся к графику
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление линий тренда</returns>
        public static IEnumerable<IChartTrendLine> GetTrendlines(this IChart chart) {
            return chart.TrendLines.Children;
        }
        /// <summary>
        ///     Установка заголовка графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="caption">Заголовок</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetCaption(this IChart chart, string caption) {
            return chart.Set<IChart>(FusionChartApi.Chart_Caption, caption);
        }
        /// <summary>
        ///     Получение заголовка графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Заголовок графика</returns>
        public static string GetCaption(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_Caption);
        }
        /// <summary>
        ///     Установка подзаголовка графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="subCaption">Подзаголовок</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetSubCaption(this IChart chart, string subCaption) {
            return chart.Set<IChart>(FusionChartApi.Chart_SubCaption, subCaption);
        }
        /// <summary>
        ///     Получение подзаголовка графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Подзаголовок графика</returns>
        public static string GetSubCaption(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_SubCaption);
        }
        /// <summary>
        ///     Установка отступа заголовка
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="captionPadding">Величина отступа</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetCaptionPadding(this IChart chart, decimal captionPadding) {
            return chart.Set<IChart>(FusionChartApi.Chart_CaptionPadding, captionPadding);
        }
        /// <summary>
        ///     Установка имени серии для датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <param name="seriesName">Имя серии</param>
        /// <returns>Замыкание на датасет</returns>
        public static IChartDataset SetSeriesName(this IChartDataset chartDataset, string seriesName) {
            return chartDataset.Set<IChartDataset>(FusionChartApi.Dataset_SeriesName, seriesName);
        }
        /// <summary>
        ///     Установка количества сторон якоря вершины дасатета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <param name="anchorSides">Количество сторон якоря</param>
        /// <returns>Замыкание на датасет</returns>
        public static IChartDataset SetAnchorSides(this IChartDataset chartDataset, int anchorSides) {
            return chartDataset.Set<IChartDataset>(FusionChartApi.Dataset_AnchorSides, anchorSides);
        }
        /// <summary>
        ///     Установка радиус якоря вершины дасатета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <param name="anchorRadius">Радиус якоря</param>
        /// <returns>Замыкание на датасет</returns>
        public static IChartDataset SetAnchorRadius(this IChartDataset chartDataset, int anchorRadius) {
            return chartDataset.Set<IChartDataset>(FusionChartApi.Dataset_AnchorRadius, anchorRadius);
        }
        /// <summary>
        ///     Установка цвета линии датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <param name="color">Цвет</param>
        /// <returns>Замыкание на датасет</returns>
        public static IChartDataset SetColor(this IChartDataset chartDataset, string color) {
            return chartDataset.Set<IChartDataset>(FusionChartApi.Dataset_Color, color);
        }
        /// <summary>
        ///     Установка типа графика в конфиг
        /// </summary>
        /// <param name="chartConfig">Представление конфига</param>
        /// <param name="type">Тип графика</param>
        /// <returns>Замыкание на конфиг</returns>
        public static IChartConfig SetChartType(this IChartConfig chartConfig, FusionChartType type) {
            chartConfig.Type = type.ToString();
            return chartConfig;
        }
        /// <summary>
        ///     Установка начального значения линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <param name="startValue">Начальное значение</param>
        /// <returns>Замыкание на линию тренда</returns>
        public static IChartTrendLine SetStartValue(this IChartTrendLine trendLine, double startValue) {
            trendLine.Set(ChartDefaults.ChartLineStartValue, startValue);
            return trendLine;
        }
        /// <summary>
        ///     Получение начального значения линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <returns>Начальное значение линии тренда</returns>
        public static double GetStartValue(this IChartTrendLine trendLine) {
            return trendLine.Get<double>(ChartDefaults.ChartLineStartValue);
        }
        /// <summary>
        ///     Установка цвета линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <param name="color">Цвет</param>
        /// <returns>Замыкание на линию тренда</returns>
        public static IChartTrendLine SetColor(this IChartTrendLine trendLine, string color) {
            trendLine.Set(ChartDefaults.ChartLineColor, color);
            return trendLine;
        }
        /// <summary>
        ///     Получение цвета линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <returns>Цвет линии тренда</returns>
        public static string GetColor(this IChartTrendLine trendLine) {
            return trendLine.Get<string>(ChartDefaults.ChartLineColor);
        }
        /// <summary>
        ///     Установка пунктирной линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <param name="dashed">Признак того, что линия пунктирная</param>
        /// <returns>Замыкание на линию тренда</returns>
        public static IChartTrendLine SetDashed(this IChartTrendLine trendLine, bool dashed) {
            trendLine.Set(ChartDefaults.ChartLineDashed, dashed);
            return trendLine;
        }
        /// <summary>
        ///     Установка пунктирной линии тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <returns>Признак того, что линия пунктирная</returns>
        public static bool GetDashed(this IChartTrendLine trendLine) {
            return trendLine.Get<bool>(ChartDefaults.ChartLineDashed);
        }
        /// <summary>
        ///     Установка подписи тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <param name="displayValue">Подпись линии тренда</param>
        /// <returns>Замыкание на линию тренда</returns>
        public static IChartTrendLine SetDisplayValue(this IChartTrendLine trendLine, string displayValue) {
            trendLine.Set(ChartDefaults.TrendLineDisplayValue, displayValue);
            return trendLine;
        }
        /// <summary>
        ///     Получение подписи тренда
        /// </summary>
        /// <param name="trendLine">Представление линии тренда</param>
        /// <returns>Подпись тренда</returns>
        public static string GetDisplayValue(this IChartTrendLine trendLine) {
            return trendLine.Get<string>(ChartDefaults.TrendLineDisplayValue);
        }
    }
}
