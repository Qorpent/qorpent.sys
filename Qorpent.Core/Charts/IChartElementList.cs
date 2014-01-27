using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Контейнерный элемент чарта
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IChartElementList<P,C> :IChartElement<P> where C :IChartElement where P : IChartElement {
        /// <summary>
        /// Дочерние элементы
        /// </summary>
        IList<C> Children { get;  }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Add(C item);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<C> items);
    }
}