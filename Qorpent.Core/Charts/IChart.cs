using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление графика
    /// </summary>
    public interface IChart : IChartXmlSource, IChartElement {
        /// <summary>
        ///     Перечисление элементов
        /// </summary>
        IChartCategories Categories { get; }
        /// <summary>
        ///     Датасеты
        /// </summary>
        IChartDatasets Datasets { get; }
        /// <summary>
        /// Набор дополнительных линий
        /// </summary>
        IChartLineSet LineSet { get; }
        /// <summary>
        /// Набор линий тренда
        /// </summary>
        IChartTrendLines TrendLines { get; }

    }
}
