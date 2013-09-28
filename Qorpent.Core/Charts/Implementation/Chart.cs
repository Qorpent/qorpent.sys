using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Представление графика
    /// </summary>
    public class Chart : IChart, IChartXmlSource {
        /// <summary>
        ///     Конфиг чарта
        /// </summary>
        private IChartConfig _config;
        /// <summary>
        ///     Внутренний список категорий
        /// </summary>
        private readonly IList<IChartElement> _categories = new List<IChartElement>();
        /// <summary>
        ///     Внутренний список датасетов
        /// </summary>
        private readonly IList<IChartElement> _datasets = new List<IChartElement>();
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
        public Chart() {
            Root = new ChartElement {
                Name = ChartDefaults.ChartElementName
            };
        }
        /// <summary>
        ///     Представление графика
        /// </summary>
        /// <param name="root">Представление корневого элемента</param>
        public Chart(IChartElement root) {
            Root = root;
        }
        /// <summary>
        ///     Представление графика
        /// </summary>
        /// <param name="config">Конфиг чарта</param>
        public Chart(IChartConfig config) {
            _config = config;
        }
        /// <summary>
        ///     Представление графика
        /// </summary>
        /// <param name="config">Конфиг чарта</param>
        /// <param name="root">Представление корневого элемента</param>
        public Chart(IChartConfig config, IChartElement root) {
            _config = config;
            Root = root;
        }
        /// <summary>
        ///     Добавление категории
        /// </summary>
        /// <param name="category">Представление категории</param>
        public void AddCategory(IChartElement category) {
            category.SetParent(Root);
            _categories.Add(category);
        }
        /// <summary>
        ///     Добавление датасета
        /// </summary>
        /// <param name="dataset">Представление датасета</param>
        public void AddDataset(IChartElement dataset) {
            dataset.SetParent(Root);
            _datasets.Add(dataset);
        }
        /// <summary>
        ///     Разрисовывает структуру в XML-представление
        /// </summary>
        /// <returns></returns>
        public XElement DrawStructure() {
            var root = Root.DrawStructure();

            foreach (var chartElement in Categories) {
                root.Add(chartElement.DrawStructure());
            }

            foreach (var chartElement in Datasets) {
                root.Add(chartElement.DrawStructure());
            }

            return root;
        }
        /// <summary>
        ///     Возвращает текущий конфиг
        /// </summary>
        /// <returns>Конфиг чарта</returns>
        public IChartConfig GetConfig() {
            return _config;
        }
        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            return DrawStructure();
        }
        /// <summary>
        ///     Поднимает представление <see cref="IChart"/> из переданног оконфига
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Настроенный по конфигу экземпляр <see cref="IChart"/></returns>
        public static IChart Initialize(IChartConfig chartConfig) {
            var chart = new Chart(chartConfig);

            foreach (var dataset in chartConfig.Datasets) {
                chart.AddDataset(dataset);
            }

            foreach (var category in chartConfig.Categories) {
                chart.AddCategory(category);
            }

            return chart;
        }
    }
}
