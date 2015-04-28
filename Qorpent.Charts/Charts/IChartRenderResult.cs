using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public interface IChartRenderResult {
        /// <summary>
        ///     Исходное представление чарта
        /// </summary>
        IChart Chart { get; }
        /// <summary>
        ///     Исходный конфиг чарта
        /// </summary>
        IChartConfig ChartConfig { get; }
        /// <summary>
        ///     Возвращает представление отрендеренного чарта в виде XML
        /// </summary>
        /// <returns>XML-представление отрендеренного чарта</returns>
        XElement AsXml();
        /// <summary>
        ///     Возвращает представление отрендеренного чарта в виде Json
        /// </summary>
        /// <returns>XML-представление отрендеренного чарта</returns>
        string AsJson();
    }
}
