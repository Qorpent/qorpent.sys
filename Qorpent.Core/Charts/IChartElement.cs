using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public interface IChartElement : IConfig {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        IChartElement Parent { get; }
        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        IList<IChartElement> Children { get; }
        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        void SetParent(IChartElement parent);

    }
}
