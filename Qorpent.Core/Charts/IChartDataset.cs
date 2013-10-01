using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Набор данных чарта
    /// </summary>
    public interface IChartDataset:IChartElementList<IChartDataItem> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Add(IChartDataItem item);
    }
}