using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public interface IChartElement {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        IChartElement Parent { get; }
        /// <summary>
        ///     Имя элемента
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Атрибуты
        /// </summary>
        IEnumerable<IChartAttribute> Attributes { get; }
        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        IEnumerable<IChartElement> Childs { get; }
        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        void SetParent(IChartElement parent);
        /// <summary>
        ///     Разрисовка структуры
        /// </summary>
        /// <returns></returns>
        XElement DrawStructure();
    }
}
