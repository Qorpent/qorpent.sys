using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IChartRender),Name = "fusion.chart.render")]    
    public class FusionChartRender:ServiceBase,IChartRender {
        private IChart _chart;
        private IChartConfig _config;

        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            var realConfig = chartConfig ?? _config;
            var fusion = _chart.AsFusion(realConfig);
            var result = fusion.GetXmlElement();
            foreach (var ds in _chart.Datasets.AsList) {
                foreach (var s in ds.AsList) {
                    var fusset = s.AsFusion(realConfig);
                    result.Add(fusset.GetXmlElement());
                }
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
        /// <param name="chart">Представление графика</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        public IChartRender Initialize(IChart chart, IChartConfig chartConfig) {
            _chart = chart;
            _config = chartConfig;
            return this;
        }
    }
}