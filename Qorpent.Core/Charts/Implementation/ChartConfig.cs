using Qorpent.Config;
using System.Collections.Generic;
namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Имплементация конфига чарта
    /// </summary>
    public class ChartConfig : ConfigBase, IChartConfig {
        /// <summary>
        ///     Датасеты
        /// </summary>
        public IEnumerable<IChartElement> Datasets {
            get { return Get <List<IChartElement>>(ChartDefaults.DatasetElementName); }
            set { Set(ChartDefaults.DatasetElementName, value); }
        }
        /// <summary>
        ///     Категории
        /// </summary>
        public IEnumerable<IChartElement> Categories {
            get { return Get<List<IChartElement>>(ChartDefaults.CategoryElementName); }
            set { Set(ChartDefaults.CategoryElementName, value); }
        }
    }
}
