using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IChartRender),Name = "fusion.chart.render")]    
    public class FusionChartRender:ServiceBase,IChartRender {
        private IChart _chart;
        /// <summary>
        ///     Внутренний экземпляр конфига рендера чартов
        /// </summary>
        private IChartConfig _config;

        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="config">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig config) {
            var realConfig = config ?? _config;
            var result = _chart.AsFusion(realConfig).GetXmlElement();
            var rootElement = result;

            var normalizer = FusionChartNormalizer.Create(realConfig);
            normalizer.Normalize(_chart);

            result.SetAttributeValue(FusionChartApi.YAxisMinValue, _chart.Get<double>(FusionChartApi.YAxisMinValue));
            result.SetAttributeValue(FusionChartApi.YAxisMaxValue, _chart.Get<double>(FusionChartApi.YAxisMaxValue));
            result.SetAttributeValue(FusionChartApi.Chart_LegendPosition, _chart.Get<string>(FusionChartApi.Chart_LegendPosition));

            foreach (var ds in _chart.Datasets.Children) {
                if (IsMultiserial(_chart)) {
                    var dataset = new XElement("dataset");
                    dataset.SetAttributeValue(FusionChartApi.Dataset_SeriesName, ds.Get<string>(FusionChartApi.Dataset_SeriesName));
                    dataset.SetAttributeValue(FusionChartApi.Dataset_Color, ds.Get<string>(FusionChartApi.Dataset_Color));
                    result.Add(dataset);
                    rootElement = dataset;
                }

                foreach (var s in ds.Children) {
                    var fusset = s.AsFusion(realConfig);
                    rootElement.Add(fusset.GetXmlElement());
                }
            }

            if (
                (IsMultiserial(_chart))
                    &&
                (_chart.Categories.Children.Any())
            ) {
                result.Add(RenderCategories(_chart));
            }

            if (_chart.TrendLines.Children.Any()) {
                result.Add(RenderTrendLines(_chart));
            }

            return result;
        }
        /// <summary>
        ///     Разрисовка категорий графика
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private XElement RenderCategories(IChart chart) {
            return new XElement("categories", chart.Categories.Children.Select(RenderCategory));
        }
        /// <summary>
        ///     Разрисовка единичной категории по конфигу
        /// </summary>
        /// <param name="categoryConfig">Конфиг категории</param>
        /// <returns></returns>
        private XElement RenderCategory(IConfig categoryConfig) {
            var category = new XElement("category");
            category.SetAttributeValue(FusionChartApi.Set_Label, categoryConfig.Get<string>(FusionChartApi.Set_Label));
            return category;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private XElement RenderTrendLines(IChart chart) {
            return new XElement("trendLines", chart.TrendLines.Children.Select(RenderLine));
        }
        /// <summary>
        ///     Разрисовка линии по её конфигу
        /// </summary>
        /// <param name="lineConfig"></param>
        /// <returns></returns>
        private XElement RenderLine(IConfig lineConfig) {
            var line = new XElement("line");

            line.SetAttributeValue(FusionChartApi.Line_StartValue, lineConfig.Get<double>(ChartDefaults.ChartLineStartValue));
            line.SetAttributeValue("color", lineConfig.Get<string>(ChartDefaults.ChartLineColor));
            line.SetAttributeValue(FusionChartApi.Line_DisplayValue, lineConfig.Get<string>(ChartDefaults.TrendLineDisplayValue));

            if (lineConfig.Get<bool>(ChartDefaults.ChartLineDashed)) {
                line.SetAttributeValue(FusionChartApi.Line_Dashed, 1);
            }

            return line;
        }
        /// <summary>
        ///     Собирает из переданного конфига чарта его XML-представление
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChartXmlSource"/></returns>
        public IChartXmlSource GenerateChartXmlSource(IChartConfig chartConfig) {
            return this;
        }
        /// <summary>
        ///     Дошлифовывает полученное представление чарта в XML-формате
        ///     до максимально идеального
        /// </summary>
        /// <param name="chartXml">Представление чарта в XML</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарта</returns>
        public XElement RefactorChartXml(XElement chartXml, IChartConfig chartConfig) {
            return chartXml;
        }
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <param name="config">Конфиг рендера чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        public IChartRender Initialize(IChart chart, IChartConfig config) {
            _chart = chart;
            _config = config;
            return this;
        }
        /// <summary>
        ///     Отрендерить чарт по переданному представлению и конфигу
        /// </summary>
        /// <param name="chartConfig">Представления конфига чарта</param>
        /// <returns>XML-представление отрендеренного чарта</returns>
        public IChartRenderResult RenderChart(IChartConfig chartConfig) {
            var renderResult = new ChartRenderResult(_chart, chartConfig);

            return renderResult;
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