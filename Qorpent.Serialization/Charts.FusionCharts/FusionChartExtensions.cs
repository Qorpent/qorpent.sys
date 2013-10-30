using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils;
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
            if (chart.Config == null) {
                return false;
            }

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
        ///     Установка отступа значения чарта
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="valuePadding">Отступ</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetValuePadding(this IChart chart, int valuePadding) {
            return chart.Set<IChart>(FusionChartApi.Chart_ValuePadding, valuePadding);
        }
        /// <summary>
        ///     Получение отступа значения чарта
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Отступ</returns>
        public static int GetValuePadding(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_ValuePadding);
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
        ///     Возвращает тип графика
        /// </summary>
        /// <param name="chart">Представлине графика</param>
        /// <returns>Тип графика</returns>
        public static FusionChartType GetChartType(this IChart chart) {
            chart.EnsureConfig();
            return chart.Config.Type.To<FusionChartType>();
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
        ///     Возвращает количество категорий графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Количество категорий графика</returns>
        public static int GetLabelCount(this IChart chart) {
            return (chart.Categories.Children != null) ? chart.Categories.Children.Count : chart.Datasets.Children.SelectMany(_ => _.Children).Count();
        }
        /// <summary>
        ///     Установка цвета рамки графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="borderColor">Цвет рамки графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetBorderColor(this IChart chart, string borderColor ) {
            return chart.Set<IChart>(FusionChartApi.Chart_BorderColor, borderColor);
        }
        /// <summary>
        ///     Получение цвета рамки графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Цвет рамки графика</returns>
        public static string GetBorderColor(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_BorderColor);
        }
        /// <summary>
        ///     Установка ширины рамки графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="borderThickness">Ширина рамки графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetBorderThickness(this IChart chart, int borderThickness) {
            return chart.Set<IChart>(FusionChartApi.Chart_BorderThickness, borderThickness);
        }
        /// <summary>
        ///     Получение ширины рамки графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Ширина рамки графика</returns>
        public static int GetBorderThickness(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_BorderThickness);
        }
        /// <summary>
        ///     Установка цвета рамки канвы
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="borderColor">Цвет рамки графика</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetCanvasBorderColor(this IChart chart, string borderColor) {
            return chart.Set<IChart>(FusionChartApi.Chart_CanvasBorderColor, borderColor);
        }
        /// <summary>
        ///     Получение цвета рамки канвы
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Цвет рамки графика</returns>
        public static string GetCanvasBorderColor(this IChart chart) {
            return chart.Get<string>(FusionChartApi.Chart_CanvasBorderColor);
        }
        /// <summary>
        ///     Установка ширины рамки канвы
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="canvasBorderThickness">Ширина рамки канвы</param>
        /// <returns>Замыкание на представление графика</returns>
        public static IChart SetCanvasBorderThickness(this IChart chart, int canvasBorderThickness) {
            return chart.Set<IChart>(FusionChartApi.Chart_CanvasBorderThickness, canvasBorderThickness);
        }
        /// <summary>
        ///     Получение ширины рамки канвы
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Ширина рамки канвы</returns>
        public static int GetCanvasBorderThickness(this IChart chart) {
            return chart.Get<int>(FusionChartApi.Chart_CanvasBorderThickness);
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
        ///     Получение имени серии для датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <returns>Имя серии</returns>
        public static string GetSeriesName(this IChartDataset chartDataset) {
            return chartDataset.Get<string>(FusionChartApi.Dataset_SeriesName);
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
        ///     Получение количества сторон якоря вершины дасатета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <returns>Количество сторон якоря</returns>
        public static int GetAnchorSides(this IChartDataset chartDataset) {
            return chartDataset.Get<int>(FusionChartApi.Dataset_AnchorSides);
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
        ///     Получение радиуса якоря вершины дасатета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <returns>Радиус якоря</returns>
        public static string GetAnchorRadius(this IChartDataset chartDataset) {
            return chartDataset.Get<string>(FusionChartApi.Dataset_AnchorRadius);
        }
        /// <summary>
        ///     Установка цвета линии датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <param name="color">Цвет</param>
        /// <returns>Замыкание на датасет</returns>
        public static IChartDataset SetColor(this IChartDataset chartDataset, string color) {
            return chartDataset.Set<IChartDataset>(FusionChartApi.Dataset_Color, new Hex(color));
        }
        /// <summary>
        ///     Установка цвета линии датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <returns>Цвет</returns>
        public static string GetColor(this IChartDataset chartDataset) {
            return chartDataset.GetColorAsHex() != null ? chartDataset.GetColorAsHex().ToHex() : "";
        }
        /// <summary>
        ///     Установка цвета линии датасета
        /// </summary>
        /// <param name="chartDataset">Представление датасета</param>
        /// <returns>Цвет</returns>
        public static Hex GetColorAsHex(this IChartDataset chartDataset) {
            return chartDataset.Get<Hex>(FusionChartApi.Dataset_Color);
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
        ///     Установка длины графика
        /// </summary>
        /// <param name="chartConfig">Представление конфига</param>
        /// <param name="width">Длина</param>
        /// <returns>Замыкание на конфиг</returns>
        public static IChartConfig SetWidth(this IChartConfig chartConfig, double width) {
            chartConfig.Width = width.ToString();
            return chartConfig;
        }
        /// <summary>
        ///     Установка высоты графика
        /// </summary>
        /// <param name="chartConfig">Представление конфига</param>
        /// <param name="height">Высота</param>
        /// <returns>Замыкание на конфиг</returns>
        public static IChartConfig SetHeight(this IChartConfig chartConfig, double height) {
            chartConfig.Height = height.ToString();
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
        /// <summary>
        ///     Установка значения сету
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <param name="value">Значение</param>
        /// <returns>Замыкание на сет</returns>
        public static IChartSet SetValue(this IChartDataItem chartSet, decimal value) {
            return chartSet.Set<IChartSet>(FusionChartApi.Set_Value, value);
        }
        /// <summary>
        ///     Получение значения сета
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <returns>Замыкание на сет</returns>
        public static decimal GetValue(this IChartDataItem chartSet) {
            return chartSet.Get<decimal>(FusionChartApi.Set_Value);
        }
        /// <summary>
        ///     Получение значения сета
        /// </summary>
        /// <param name="chartSet">Представление сета</param>
        /// <returns>Замыкание на сет</returns>
        public static T GetValue<T>(this IChartDataItem chartSet) {
            return chartSet.Get<T>(FusionChartApi.Set_Value).To<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="showLabel"></param>
        /// <returns></returns>
        public static IChartDataItem SetShowValue(this IChartDataItem dataItem, bool showLabel) {
            return dataItem.Set<IChartDataItem>(FusionChartApi.Set_ShowValue, showLabel);
        }
        /// <summary>
        ///     Установа паддинга канвы чарта
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <param name="canvasPadding">Паддинг канвы</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart SetCavasPadding(this IChart chart, double canvasPadding) {
            return chart.Set<IChart>(FusionChartApi.Chart_CanvasPadding, canvasPadding);
        }
        /// <summary>
        ///     Получение паддинга канвы чарта
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Паддинг канвы</returns>
        public static double GetCavasPadding(this IChart chart) {
            return chart.Get<double>(FusionChartApi.Chart_CanvasPadding);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        public static bool GetShowValue(this IChartDataItem dataItem) {
            return dataItem.Get<bool>(FusionChartApi.Set_ShowValue);
        }
        /// <summary>
        ///     Возвращает дельту: разницу между максимальным и минимальным значением
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Дельта</returns>
        public static double GetDelta(this IChart chart) {
            return chart.GetYAxisMaxValue() - chart.GetYAxisMinValue();
        }
        /// <summary>
        ///     Возвращает перечисление значений датасетов в виде плоского списка
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление значений датасетов в виде плоского списка</returns>
        public static IEnumerable<double> GetLinearDatasetValues(this IChart chart) {
            return chart.Datasets.Children.SelectMany(
                _ => _.Children
            ).Select(
                _ => _.Get<double>(FusionChartApi.Set_Value)
            );
        }
        /// <summary>
        ///     Возвращает максимальное значение из всех сетов датасетов графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Максимальное значение из всех сетов датасетов графика</returns>
        public static double GetMaxDatasetSetValue(this IChart chart) {
            return chart.GetLinearDatasetValues().Max();
        }
        /// <summary>
        ///     Возвращает минимальное значение из всех сетов датасетов графика
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Минимальное значение из всех сетов датасетов графика</returns>
        public static double GetMinDatasetSetValue(this IChart chart) {
            return chart.GetLinearDatasetValues().Min();
        }
        /// <summary>
        ///     Возвращает перечисление значений трендлайнов, относящихся к чарту
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Перечисление значений трендлайнов, относящихся к чарту</returns>
        public static IEnumerable<double> GetTrendlinesValues(this IChart chart) {
            return chart.GetTrendlines().Select(
                _ => _.Get<double>(ChartDefaults.ChartLineStartValue)
            );
        }
        /// <summary>
        ///     Возвращает минимальное значение линии тренда из всех существующих
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Минимальное значение линии тренда из всех существующих</returns>
        public static double GetMinTrendlineValue(this IChart chart) {
            return chart.GetTrendlinesValues().Min();
        }
        /// <summary>
        ///     Возвращает максимальное значение линии тренда из всех существующих
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Максимальное значение линии тренда из всех существующих</returns>
        public static double GetMaxTrendlineValue(this IChart chart) {
            return chart.GetTrendlinesValues().Max();
        }
        /// <summary>
        ///     Возвращает минимальное значение всего чарта относительно оси Y с учётом трендлайнов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Минимальное значение всего чарта относительно оси Y с учётом трендлайнов</returns>
        public static double GetYMinValueWholeChart(this IChart chart) {
            return chart.TrendLines.Children.Any() ? chart.GetMinDatasetSetValue().Minimal(chart.GetMinTrendlineValue()) : chart.GetMinDatasetSetValue();
        }
        /// <summary>
        ///     Возвращает минимальное значение всего чарта относительно оси Y с учётом трендлайнов
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Минимальное значение всего чарта относительно оси Y с учётом трендлайнов</returns>
        public static double GetYMaxValueWholeChart(this IChart chart) {
            return chart.TrendLines.Children.Any() ? chart.GetMaxDatasetSetValue().Maximal(chart.GetMaxTrendlineValue()) : chart.GetMaxDatasetSetValue();
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <returns>Замыкание на нормализованный чарт</returns>
        public static IChart Normalize(this IChart chart) {
            return new FusionChartsNormalizeFactory().Normalize(chart);
        }
        /// <summary>
        ///     Проверяет наличие датасетов в чарте
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Признак наличия датасетов в чарте</returns>
        public static bool DatasetsExists(this IChart chart) {
            return chart.Datasets.Children.Any();
        }
        /// <summary>
        ///     Проверяет наличие конфига и создат его в случа отсутствия
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Замякание на представление чарта</returns>
        public static IChartConfig EnsureConfig(this IChart chart) {
            if (chart.Config == null) {
                chart.SetConfig(new ChartConfig());
            }

            return chart.Config;
        }
    }
}
