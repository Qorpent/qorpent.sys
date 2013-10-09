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

            FusionChartNormalizer.Create(realConfig).Normalize(_chart);

            SetAttrs(_chart, result, new[] {
                FusionChartApi.YAxisMinValue,
                FusionChartApi.YAxisMaxValue,
                FusionChartApi.Chart_LegendPosition
            });

            RenderDatasets(_chart, realConfig, result);

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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="keys"></param>
        private void SetAttrs(IConfig source, XElement target, IEnumerable<string> keys) {
            keys.DoForEach(_ => target.SetAttr(_, source.Get<object>(_)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="chartConfig"></param>
        /// <param name="result"></param>
        private void RenderDatasets(IChart chart, IChartConfig chartConfig, XElement result) {
            foreach (var ds in _chart.Datasets.Children) {
                var xml = RenderDataset(ds, chartConfig);
                result.Add(IsMultiserial(_chart) ? new[] { xml } : xml.Elements());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private XElement RenderDataset(IChartDataset dataset, IChartConfig config) {
            var xml = new XElement("dataset", RenderDatasetSets(dataset, config));

            SetAttrs(dataset, xml, new[] {
                FusionChartApi.Dataset_SeriesName,
                FusionChartApi.Dataset_Color,
                FusionChartApi.Chart_AnchorRadius,
                FusionChartApi.Dataset_AnchorSides
            });

            return xml;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private IEnumerable<XElement> RenderDatasetSets(IChartDataset dataset, IChartConfig config) {
            return dataset.Children.Select(_ => _.AsFusion(config).GetXmlElement());
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
            category.SetAttr(FusionChartApi.Set_Label, categoryConfig.Get<string>(FusionChartApi.Set_Label));
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

            line.Attr(FusionChartApi.Line_StartValue, lineConfig.Get<double>(ChartDefaults.ChartLineStartValue).ToString());
            line.Attr("color", lineConfig.Get<string>(ChartDefaults.ChartLineColor));
            line.Attr(FusionChartApi.Line_DisplayValue, lineConfig.Get<string>(ChartDefaults.TrendLineDisplayValue));

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