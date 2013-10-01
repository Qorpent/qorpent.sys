using System;
using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public class ChartRenderResult : IChartRenderResult {
        /// <summary>
        ///     Исходное представление чарта
        /// </summary>
        public IChart Chart { get; private set; }
        /// <summary>
        ///     Исходный конфиг чарта
        /// </summary>
        public IChartConfig ChartConfig { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="chartConfig">Исходный конфиг чарта</param>
        public ChartRenderResult(IChart chart, IChartConfig chartConfig) {
            Chart = chart;
            ChartConfig = chartConfig;
        }
        /// <summary>
        ///     Возвращает представление отрендеренного чарта в виде XML
        /// </summary>
        /// <returns>XML-представление отрендеренного чарта</returns>
        public XElement AsXml() {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Возвращает представление отрендеренного чарта в виде Json
        /// </summary>
        /// <returns>XML-представление отрендеренного чарта</returns>
        public string AsJson() {
            throw new NotImplementedException();
        }
    }
}
