using System;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление графика
    /// </summary>
    public interface IChart : IChartXmlSource, IChartElementList<IChart,IChartRootElement> {
        /// <summary>
        ///     Перечисление элементов
        /// </summary>
        IChartCategories Categories { get; }
        /// <summary>
        ///     Датасеты
        /// </summary>
        IChartDatasets Datasets { get; }
        /// <summary>
        ///     Набор дополнительных линий
        /// </summary>
        IChartLineSet LineSet { get; }
        /// <summary>
        ///     Набор линий тренда
        /// </summary>
        IChartTrendLines TrendLines { get; }
        /// <summary>
        ///     Конфиг чарта
        /// </summary>
        IChartConfig Config { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IChartRootElement:IChartElement<IChart> { }
    /// <summary>
    /// Абстракция корневого элемента чарта
    /// </summary>
    public interface IChartRootElement<C> : IChartRootElement,IChartElementList<IChart, C> where C : IChartElement {}
}
