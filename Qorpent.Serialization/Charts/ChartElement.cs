using System.Collections.Generic;
using Qorpent.Charts.FusionCharts;
using Qorpent.Config;

namespace Qorpent.Charts {


    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public abstract class ChartElement<P> : ConfigBase, IChartElement<P> where P : IChartElement {
        /// <summary>
        /// Элементы чарта не наследуют атрибутов друг друга
        /// </summary>
        public ChartElement() {
            UseInheritance = false;
            Set(ChartDefaults.ChartElementChilds,new List<P>());
        }
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public P Parent {
            get { return Get<P>(ChartDefaults.ChartElementParentProperty); }
            private set { Set(ChartDefaults.ChartElementParentProperty, value); }
        }

        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public string ElementName {
            get { return Get<string>(ChartDefaults.ChartElementName); }
            protected set { Set(ChartDefaults.ChartElementName, value); }
        }

        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        public void SetParentElement(IChartElement parent) {
            Parent =(P) parent;
        }
       
      
    }
}
