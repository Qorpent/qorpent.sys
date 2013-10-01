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
        ///     Устанавливает имя элемента
        /// </summary>
        /// <param name="name">Имя элемента</param>
        void SetName(string name);
        /// <summary>
        ///     Разрисовка структуры
        /// </summary>
        /// <returns></returns>
        XElement ToXml();
    }
}
