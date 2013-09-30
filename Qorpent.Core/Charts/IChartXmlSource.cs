using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление XML-источника чарта
    /// </summary>
    public interface IChartXmlSource {
        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        XElement GenerateChartXml(IChartConfig chartConfig);
    }
}
