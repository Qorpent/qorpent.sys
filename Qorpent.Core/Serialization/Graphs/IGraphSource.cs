using System.Xml.Linq;

namespace Qorpent.Serialization.Graphs {
    /// <summary>
    /// Источник графа
    /// </summary>
    public interface IGraphSource {
        /// <summary>
        /// Вызывает процедуру построения графа
        /// </summary>
        /// <returns></returns>
        IGraphConvertible BuildGraph(GraphOptions options);
        /// <summary>
        /// Выполнение пост-обработки SVG с графом
        /// </summary>
        /// <param name="currentSvg"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        XElement PostprocessGraphSvg(XElement currentSvg, GraphOptions options);
    }
}