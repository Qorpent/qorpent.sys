using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Представление графика
    /// </summary>
    public class Chart : IChart {
        /// <summary>
        ///     Внутренний список категорий
        /// </summary>
        private readonly IList<IChartElement> _categories;
        /// <summary>
        ///     Внутренний список датасетов
        /// </summary>
        private readonly IList<IChartElement> _datasets;
        /// <summary>
        ///     Корневой элемент
        /// </summary>
        public IChartElement Root { get; private set; }
        /// <summary>
        ///     Перечисление элементов
        /// </summary>
        public IEnumerable<IChartElement> Categories {
            get { return _categories; }
        }
        /// <summary>
        ///     Датасеты
        /// </summary>
        public IEnumerable<IChartElement> Datasets {
            get { return _datasets; }
        }
        /// <summary>
        ///     Представление графика
        /// </summary>
        /// <param name="root">Представление корневого элемента</param>
        public Chart(IChartElement root) {
            Root = root;

            _categories = new List<IChartElement>();
            _datasets = new List<IChartElement>();
        }
        /// <summary>
        ///     Добавление категории
        /// </summary>
        /// <param name="category">Представление категории</param>
        public void AddCategory(IChartElement category) {
            _categories.Add(category);
        }
        /// <summary>
        ///     Добавление датасета
        /// </summary>
        /// <param name="dataset">Представление датасета</param>
        public void AddDataset(IChartElement dataset) {
            _datasets.Add(dataset);
        }
        /// <summary>
        ///     Разрисовывает структуру в XML-представление
        /// </summary>
        /// <returns></returns>
        public XElement DrawStructure() {
            throw new NotImplementedException();
        }
    }
}
