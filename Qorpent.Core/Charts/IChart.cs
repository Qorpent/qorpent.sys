using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление графика
    /// </summary>
    public interface IChart : IChartXmlSource, IConfig {
        /// <summary>
        ///     Корневой элемент
        /// </summary>
        IChartElement Root { get; }
        /// <summary>
        ///     Перечисление элементов
        /// </summary>
        IEnumerable<IChartElement> Categories { get; }
        /// <summary>
        ///     Датасеты
        /// </summary>
        IEnumerable<IChartElement> Datasets { get; }
        /// <summary>
        ///     Добавление категории
        /// </summary>
        /// <param name="category">Представление категории</param>
        void AddCategory(IChartElement category);
        /// <summary>
        ///     Добавление датасета
        /// </summary>
        /// <param name="dataset">Представление датасета</param>
        void AddDataset(IChartElement dataset);
    }
}
