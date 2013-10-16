using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IChartRender),Name = "fusion.chart.render")]    
    public class FusionChartRender : ServiceBase,IChartRender {
        /// <summary>
        ///     Нормалайзер чартов
        /// </summary>
        [Inject]
        public IСhartNormalizer ChartNormalizer { get; set; }
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

            if (ChartNormalizer == null) {
                ChartNormalizer = new FusionChartNormalizer();
            }

            ChartNormalizer.Initialize(realConfig);
            ChartNormalizer.Normalize(_chart);

            result.SetAttr(FusionChartApi.Chart_XAxisName, _chart.GetXAxisName());
            result.SetAttr(FusionChartApi.Chart_YAxisName, _chart.GetYAxisName());
            result.SetAttr(FusionChartApi.Chart_YAxisMinValue, _chart.GetYAxisMinValue());
            result.SetAttr(FusionChartApi.Chart_YAxisMaxValue, _chart.GetYAxisMaxValue());
            result.SetAttr(FusionChartApi.Chart_BgColor, _chart.GetBgColor());
            result.SetAttr(FusionChartApi.Chart_Alpha, _chart.GetAlpha());
            result.SetAttr(FusionChartApi.Chart_DivIntervalHints, _chart.GetDivIntervalHints());
            result.SetAttr(FusionChartApi.Chart_DivLineAlpha, _chart.GetDivLineAlpha());
            result.SetAttr(FusionChartApi.Chart_ShowAlternateHGridColor, _chart.GetShowAlternateHGridColor() ? 1 : 0);
            result.SetAttr(FusionChartApi.Chart_ChartOrder, _chart.GetChartOrder());
            result.SetAttr(FusionChartApi.Chart_NumDivLines, _chart.GetNumDivLines() != 0 ? _chart.GetNumDivLines().ToString() : null);
            result.SetAttr(FusionChartApi.Chart_ValuePadding, _chart.GetValuePadding() != 0 ? _chart.GetValuePadding().ToString() : null);
            result.SetAttr(FusionChartApi.Chart_BorderColor, _chart.GetBorderColor());
            result.SetAttr(FusionChartApi.Chart_BorderThickness, _chart.GetBorderThickness() != 0 ? _chart.GetBorderThickness().ToString() : null);

            SetAttrs(_chart, result, new[] {
                FusionChartApi.Chart_LegendPosition,
                FusionChartApi.Chart_FormatNumber,
                FusionChartApi.Chart_FormatNumberScale
            });

            RenderDatasets(_chart, realConfig, result);

            if (
                (_chart.IsMultiserial() || _chart.IsCombined())
                    &&
                (_chart.GetCategories().Any())
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
        private void SetAttrs(IChartElement source, XElement target, IEnumerable<string> keys) {
            keys.DoForEach(_ => target.SetAttr(_, source.Get<object>(_)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="chartConfig"></param>
        /// <param name="result"></param>
        private void RenderDatasets(IChart chart, IChartConfig chartConfig, XElement result) {
            foreach (var xml in chart.GetDatasets().Select(ds => RenderDataset(ds, chartConfig))) {
                result.Add(chart.IsMultiserial() || chart.IsCombined() ? new[] { xml } : xml.Elements());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private XElement RenderDataset(IChartDataset dataset, IChartConfig config) {
            return new XElement(FusionChartApi.Dataset, RenderDatasetSets(dataset, config))
                .SetAttr(FusionChartApi.Set_Color, dataset.GetColor())
                .SetAttr(FusionChartApi.Dataset_SeriesName, dataset.GetSeriesName())
                .SetAttr(FusionChartApi.Chart_AnchorRadius, dataset.GetAnchorRadius())
                .SetAttr(FusionChartApi.Chart_AnchorSides, dataset.GetAnchorSides());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private IEnumerable<XElement> RenderDatasetSets(IChartDataset dataset, IChartConfig config) {
            return dataset.Children.Select(_ => _.AsFusion(config).GetXmlElement().SetAttr(FusionChartApi.Set_ShowValue, _.GetShowValue() ? "1" : "0"));
        }
        /// <summary>
        ///     Разрисовка категорий графика
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private XElement RenderCategories(IChart chart) {
            return new XElement(FusionChartApi.Categories, chart.GetCategories().Select(RenderCategory));
        }
        /// <summary>
        ///     Разрисовка единичной категории по конфигу
        /// </summary>
        /// <param name="categoryConfig">Конфиг категории</param>
        /// <returns></returns>
        private XElement RenderCategory(IChartCategory categoryConfig) {
            var category = new XElement(FusionChartApi.Category);
            category.SetAttr(FusionChartApi.Set_Label, categoryConfig.Get<string>(FusionChartApi.Set_Label));
            return category;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private XElement RenderTrendLines(IChart chart) {
            return new XElement(FusionChartApi.TrendLines, chart.GetTrendlines().Select(RenderLine));
        }
        /// <summary>
        ///     Разрисовка линии по её конфигу
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private XElement RenderLine(IChartTrendLine line) {
            var xml = new XElement(FusionChartApi.Line);

            xml.SetAttr(FusionChartApi.Line_StartValue, line.GetStartValue());
            xml.SetAttr(FusionChartApi.Line_Color, line.GetColor());
            xml.SetAttr(FusionChartApi.Line_DisplayValue, line.GetDisplayValue());

            if (line.GetDashed()) {
                xml.SetAttributeValue(FusionChartApi.Line_Dashed, 1);
            }

            return xml;
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
    }
}