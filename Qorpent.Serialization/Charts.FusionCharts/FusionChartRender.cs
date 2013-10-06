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
            var fusion = _chart.AsFusion(realConfig);
            var result = fusion.GetXmlElement();
            var rootElement = result;

            var normalizer = FusionChartNormalizer.Create(realConfig);
            normalizer.Normalize(_chart);

            result.SetAttributeValue("yAxisMinValue", _chart.Get<double>("yAxisMinValue").ToInt());

            foreach (var ds in _chart.Datasets.Children) {
                if (IsMultiserial(_chart)) {
                    var dataset = new XElement("dataset");
                    result.Add(dataset);
                    rootElement = dataset;
                }

                foreach (var s in ds.Children) {
                    var fusset = s.AsFusion(realConfig);
                    rootElement.Add(fusset.GetXmlElement());
                }
            }

            if (IsMultiserial(_chart)) {
                if (_chart.Categories.Children.Count != 0) {
                    var categories = new XElement("categories");
                    foreach (var cat in _chart.Categories.Children) {
                        var category = new XElement("category");
                        category.SetAttributeValue(FusionChartApi.Set_Label, cat.Get<string>(FusionChartApi.Set_Label));
                        categories.Add(category);
                    }

                    result.Add(categories);
                }
            }

            var trendLines = new XElement("trendLines");
            result.Add(trendLines);

            foreach (var tl in _chart.TrendLines.Children) {
                var trendLine = new XElement("line");
                trendLine.SetAttributeValue("startValue", tl.Get<double>(ChartDefaults.ChartLineStartValue));
                trendLine.SetAttributeValue("color", tl.Get<string>(ChartDefaults.ChartLineColor));
                trendLine.SetAttributeValue("displayValue", tl.Get<string>(ChartDefaults.TrendLineDisplayValue));
                if (tl.Get<bool>(ChartDefaults.ChartLineDashed)) {
                    trendLine.SetAttributeValue("dashed", 1);
                }

                trendLines.Add(trendLine);
            }

            return result;
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