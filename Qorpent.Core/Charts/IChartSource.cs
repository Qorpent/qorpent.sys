using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public interface IChartSource {
        /// <summary>
        ///     Собирает из переданного конфига чарта его XML-представление
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChartXmlSource"/></returns>
        IChartXmlSource GenerateChartXmlSource(IChartConfig chartConfig);
        /// <summary>
        ///     Дошлифовывает полученное представление чарта в XML-формате
        ///     до максимально идеального
        /// </summary>
        /// <param name="chartXml">Представление чарта в XML</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарта</returns>
        XElement RefactorChartXml(XElement chartXml, IChartConfig chartConfig);
    }
}
