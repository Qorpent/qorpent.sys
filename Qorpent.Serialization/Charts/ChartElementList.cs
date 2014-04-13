using System.Collections.Generic;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts {
    /// <summary>
    /// Заготовка под перечислимый элемент чарта
    /// </summary>
   
    public abstract class ChartElementList<P,C> : ChartElement<P>, IChartElementList<P,C> where P : IChartElement where C : IChartElement {
        private IList<C> _children;

        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        public IList<C> Children {
            get {
                if (_children == null) {
                    _children = Get<IList<C>>(ChartDefaults.ChartElementChilds);
                    if (_children == null) {
                        _children = new List<C>();
                        Set(ChartDefaults.ChartElementChilds,_children);
                    }
                }
                return _children;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(C item) {
            if (!Children.Contains(item)) {
                item.SetParentElement(this);
                Children.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<C> items) {
            LinqExtensions.DoForEach(items, Add);
        }
    }
}