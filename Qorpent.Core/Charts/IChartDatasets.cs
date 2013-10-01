using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Коллекция наборов данных
    /// </summary>
    public interface IChartDatasets:IChartElement,IList<IChartDataset> {
    }
}