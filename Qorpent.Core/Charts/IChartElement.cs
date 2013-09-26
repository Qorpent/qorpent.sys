using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public interface IChartElement {
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
    }
}
