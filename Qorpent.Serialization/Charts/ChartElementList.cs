using System;
using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// ��������� ��� ������������ ������� �����
    /// </summary>
   
    public abstract class ChartElementList<P,C> : ChartElement<P>, IChartElementList<P,C> where P : IChartElement where C : IChartElement {
        private IList<C> _children;

        /// <summary>
        ///     �������� ��������
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
                this.Children.Add(item);
            }
        }
    }
}