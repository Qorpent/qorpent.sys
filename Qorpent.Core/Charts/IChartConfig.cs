using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    ///     Описание конфига чарта
    /// </summary>
    public interface IChartConfig {
        /// <summary>
        ///     Датасеты
        /// </summary>
        IEnumerable<IChartElement> Datasets { get; }
        /// <summary>
        ///     Категории
        /// </summary>
        IEnumerable<IChartElement> Categories { get; }
    }
}
