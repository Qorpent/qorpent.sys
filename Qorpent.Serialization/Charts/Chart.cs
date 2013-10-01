using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    /// Основной объект - сам график
    /// </summary>
    public partial class Chart : ChartElement,IChart {
        private IChartCategories _categories;
        private IChartDatasets _datasets;
        private IChartLineSet _lineSet;
        private IChartTrendLines _trendLines;

        /// <summary>
        ///     Собирается XML-представление чарата по его конфигу
        /// </summary>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>XML-представление чарата</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            var render = CreateChartRender(chartConfig);
            var xml = render.GenerateChartXmlSource(chartConfig).GenerateChartXml(chartConfig);
            xml = render.RefactorChartXml(xml,chartConfig);
            return xml;
        }

        /// <summary>
        /// Перекрыть, чтобы создать класс рендера для данного класса
        /// </summary>
        /// <param name="chartConfig"></param>
        /// <returns></returns>
        protected virtual IChartRender CreateChartRender(IChartConfig chartConfig) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Перечисление элементов
        /// </summary>
        public IChartCategories Categories {
            get {
                if (null == _categories) {
                    _categories = CreateCategoriesElement();
                    _categories.SetParent(this);
                    Children.Add(_categories);
                    Set(ChartDefaults.Categories,_categories);
                }
                return _categories;
            }
        }
        /// <summary>
        /// Перекрыть для создания элемента для категорий
        /// </summary>
        /// <returns></returns>
        protected virtual IChartCategories CreateCategoriesElement() {
            return new ChartCategories();
        }

        /// <summary>
        ///     Датасеты
        /// </summary>
        public IChartDatasets Datasets {
            get
            {
                if (null == _datasets)
                {
                    _datasets = CreateDatasetsElement();
                    _datasets.SetParent(this);
                    Children.Add(_datasets);
                    Set(ChartDefaults.Datasets, _datasets);
                }
                return _datasets;
            }
        }
        /// <summary>
        /// Создает набор элемента данных
        /// </summary>
        /// <returns></returns>
        protected virtual IChartDatasets CreateDatasetsElement() {
            return new ChartDatasets();
        }

        /// <summary>
        /// Набор дополнительных линий
        /// </summary>
        public IChartLineSet LineSet {
            get
            {
                if (null == _lineSet)
                {
                    _lineSet = CreateLinesetElement();
                    _lineSet.SetParent(this);
                    Children.Add(_lineSet);
                    Set(ChartDefaults.Datasets, _lineSet);
                }
                return _lineSet;
            }
        }
        /// <summary>
        /// Создает новый элемент лайнсетов
        /// </summary>
        /// <returns></returns>
        protected virtual IChartLineSet CreateLinesetElement() {
            return new ChartLineSet();
        }

        /// <summary>
        /// Набор линий тренда
        /// </summary>
        public IChartTrendLines TrendLines {
            get
            {
                if (null == _trendLines)
                {
                    _trendLines = CreateTrendlinesElement();
                    _trendLines.SetParent(this);
                    Children.Add(_trendLines);
                    Set(ChartDefaults.Datasets, _trendLines);
                }
                return _trendLines;
            }
        }
        /// <summary>
        /// Согздает реальный элемент Trendline
        /// </summary>
        /// <returns></returns>
        public virtual IChartTrendLines CreateTrendlinesElement() {
            return new ChartTrendLines();
        }
    }
}