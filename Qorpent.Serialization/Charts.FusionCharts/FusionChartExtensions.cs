using System.Collections.Generic;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Набор расширений для работы с FusionCharts на базе абстракций чартов
    /// </summary>
    public static class FusionChartExtensions {
        /// <summary>
        ///     Признак того, что график мультисерийный
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Признак того, что график мультисерийный</returns>
        public static bool IsMultiserial(this IChart chart) {
            return chart.Is(FusionChartGroupedType.MultiSeries);
        }
        /// <summary>
        ///     Признак того, что график односерийный
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Признак того, что график односерийный</returns>
        public static bool IsSingleSeries(this IChart chart) {
            return chart.Is(FusionChartGroupedType.SingleSeries);
        }
        /// <summary>
        ///     Признак того, что график относится к комбинированным
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Признак того, что график комбинированный</returns>
        public static bool IsCombined(this IChart chart) {
            return chart.Is(FusionChartGroupedType.Compination);
        }
        /// <summary>
        ///     Проверить, что представление графика попадает в указанную группу
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="groupedType">Тип группы</param>
        /// <returns>Признак того, что представление графика попадает в указанную группу</returns>
        public static bool Is(this IChart chart, FusionChartGroupedType groupedType) {
            var f = chart.Config.Type.To<FusionChartType>();
            if (
                (f & (FusionChartType)groupedType) == f
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
        ///     Установка фона графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="bgColor">Фон графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetBgColor(this IChart chart, string bgColor) {
            return chart.Set<IChart>(FusionChartApi.Chart_BgColor, bgColor);
        }
        /// <summary>
        ///     Получение фона графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Фон графика</returns>
        public static string GetBgColor(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_BgColor);
        }
        /// <summary>
        ///     Установка прозрачности фона графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="alpha">Прозрачность фона графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetAlpha(this IChart chart, int alpha) {
            return chart.Set<IChart>(FusionChartApi.Chart_Alpha, alpha);
        }
        /// <summary>
        ///     Получение прозрачности фона графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Прозрачность фона графика</returns>
        public static int GetAlpha(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_Alpha);
        }
        /// <summary>
        ///     Установка интервала разрисовки
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="divIntervalHints">Интервал разрисовки</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetDivIntervalHints(this IChart chart, string divIntervalHints) {
            return chart.Set<IChart>(FusionChartApi.Chart_DivIntervalHints, divIntervalHints);
        }
        /// <summary>
        ///     Получение интервала разрисовки
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Интервал разрисовки</returns>
        public static string GetDivIntervalHints(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_DivIntervalHints);
        }
        /// <summary>
        ///     Установка прозрачности фона линий тренда
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="divLineAlpha">Прозрачность фона линий тренда</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetDivLineAlpha(this IChart chart, int divLineAlpha) {
            return chart.Set<IChart>(FusionChartApi.Chart_DivLineAlpha, divLineAlpha);
        }
        /// <summary>
        ///     Получение прозрачности фона линий тренда
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Прозрачность фона линий тренда</returns>
        public static int GetDivLineAlpha(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_DivLineAlpha);
        }
        /// <summary>
        ///     Установка отображения горизонтальных чередующихся блоков линий тренда другим цветом
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="showAlternateHGridColor">Признак отрисовки</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetShowAlternateHGridColor(this IChart chart, bool showAlternateHGridColor) {
            return chart.Set<IChart>(FusionChartApi.Chart_ShowAlternateHGridColor, showAlternateHGridColor);
        }
        /// <summary>
        ///     Получение отображения горизонтальных чередующихся блоков линий тренда другим цветом
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Признак отрисовки</returns>
        public static bool GetShowAlternateHGridColor(this IChart chart) {
            return chart.Get<bool>(FusionChartApi.Chart_ShowAlternateHGridColor);
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
        ///     Установка имени X оси
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="name">Имя X оси</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetXAxisName(this IChart chart, string name) {
            return chart.Set<IChart>(FusionChartApi.Chart_XAxisName, name);
        }
        /// <summary>
        ///     Получение имени X оси
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Имя X оси</returns>
        public static string GetXAxisName(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_XAxisName);
        }
        /// <summary>
        ///     Установка минимального значения оси Y
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="minValue">Минимальное значение</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetYAxisMinValue(this IChart chart, double minValue) {
            return chart.Set<IChart>(FusionChartApi.Chart_XAxisMinValue, minValue);
        }
        /// <summary>
        ///     Получение минимального значения оси Y
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static double GetYAxisMinValue(this IChart chart) {
            return chart.Get<double>(FusionChartApi.Chart_XAxisMinValue);
        }
        /// <summary>
        ///     Установка максимального значения оси Y
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="minValue">Минимальное значение</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetYAxisMaxValue(this IChart chart, double minValue) {
            return chart.Set<IChart>(FusionChartApi.Chart_XAxisMaxValue, minValue);
        }
        /// <summary>
        ///     Получение максимального значения оси Y
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Максимальное значение оси Y</returns>
        public static double GetYAxisMaxValue(this IChart chart) {
            return chart.Get<double>(FusionChartApi.Chart_XAxisMaxValue);
        }
        /// <summary>
        ///     Установка количества дивлайнов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="numDivLines">Количество дивлайнов</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetNumDivLines(this IChart chart, int numDivLines) {
            return chart.Set<IChart>(FusionChartApi.Chart_NumDivLines, numDivLines);
        }
        /// <summary>
        ///     Получение количества дивлайнов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Количество дивлайнов</returns>
        public static int GetNumDivLines(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_NumDivLines);
        }
        /// <summary>
        ///     Установка имени Y оси
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="name">Имя Y оси</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetYAxisName(this IChart chart, string name) {
            return chart.Set<IChart>(FusionChartApi.Chart_YAxisName, name);
        }
        /// <summary>
        ///     Получение имени Y оси
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Имя Y оси</returns>
        public static string GetYAxisName(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_YAxisName);
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
        ///     Установка порядка отрисовки графиков
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="chartOrder">Порядок отрисовки графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetChartOrder(this IChart chart, string chartOrder) {
            return chart.Set<IChart>(FusionChartApi.Chart_ChartOrder, chartOrder);
        }
        /// <summary>
        ///     Получение порядка отрисовки графиков
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Порядок отрисовки графика</returns>
        public static string GetChartOrder(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_ChartOrder);
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
        /// <summary>
        ///     Установка метки к сету
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <param name="label">Метка</param>
        /// <returns>Замыкание на сет</returns>
        public static IChartSet SetLabel(this IChartSet chartSet, string label) {
            return chartSet.Set<IChartSet>(FusionChartApi.Set_Label, label);
        }
        /// <summary>
        ///     Получение метки к сету
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <returns>Метка</returns>
        public static string GetLabel(this IChartSet chartSet) {
            return chartSet.Get<string>(FusionChartApi.Set_Label);
        }
        /// <summary>
        ///     Установка значения сету
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <param name="value">Значение</param>
        /// <returns>Замыкание на сет</returns>
        public static IChartSet SetValue(this IChartSet chartSet, decimal value) {
            return chartSet.Set<IChartSet>(FusionChartApi.Set_Value, value);
        }
        /// <summary>
        ///     Получение значения сета
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <returns>Замыкание на сет</returns>
        public static decimal GetValue(this IChartSet chartSet) {
            return chartSet.Get<decimal>(FusionChartApi.Set_Value);
        }
    }
}
