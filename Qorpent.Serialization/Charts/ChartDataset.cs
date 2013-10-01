using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление датасета
    /// </summary>
    public class ChartDataset : ChartElementList<IChartDataItem>,IChartDataset {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(IChartDataItem item) {
           item.SetParent(this);
            Children.Add(item);
            RealList.Add(item);
        }
    }
}
