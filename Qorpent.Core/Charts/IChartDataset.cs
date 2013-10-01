using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Набор данных чарта
    /// </summary>
    public interface IChartDataset:IChartElement,IEnumerable<IChartDataItem> {
    }
}