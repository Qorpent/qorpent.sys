using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Ренедр чартов
    /// </summary>
    public class ChartRender : IChartRender, IChartXmlSource, IChartSource {
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        public IChartRender Initialize(IChart chart, IChartConfig chartConfig) {
            return this;
        }
        /// <summary>
        ///     Собирает из переданного конфига чарта его XML-представление
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChartXmlSource"/></returns>
        public IChartXmlSource GenerateChartXmlSource(IChartConfig chartConfig) {
            return new ChartXmlSource();
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
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            return GenerateChartXmlSource(chartConfig).GenerateChartXml(chartConfig);
        }
    }
}
