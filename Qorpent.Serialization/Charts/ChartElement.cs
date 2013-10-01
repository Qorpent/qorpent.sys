using System.Collections.Generic;
using Qorpent.Charts.FusionCharts;
using Qorpent.Config;

namespace Qorpent.Charts {


    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public abstract class ChartElement : ConfigBase, IChartElement {
        private IList<IChartElement> _children;

        /// <summary>
        /// Элементы чарта не наследуют атрибутов друг друга
        /// </summary>
        public ChartElement() : base() {
            UseInheritance = false;
            Set(ChartDefaults.ChartElementChilds,new List<IChartElement>());
        }
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public IChartElement Parent {
            get { return Get<IChartElement>(ChartDefaults.ChartElementParentProperty, this); }
            private set { Set(ChartDefaults.ChartElementParentProperty, value); }
        }

        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public string ElementName
        {
            get { return Get<string>(ChartDefaults.ChartElementName); }
            protected set { Set(ChartDefaults.ChartElementName, value); }
        }

        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        public IList<IChartElement> Children {
            get {
                return _children ?? (_children = Get<IList<IChartElement>>(ChartDefaults.ChartElementChilds));
            }

        }
        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        public void SetParent(IChartElement parent) {
            Parent = parent;
        }
       
      
    }
}
