using System.Xml.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    /// 
    /// </summary>
    public class ChartXmlSource : IChartXmlSource {
        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            return Chart.Initialize(chartConfig).GenerateChartXml(chartConfig);
        }
    }
}
