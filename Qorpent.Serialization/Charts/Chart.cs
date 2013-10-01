using System.Xml.Linq;

namespace Qorpent.Charts {
    /// <summary>
    /// �������� ������ - ��� ������
    /// </summary>
    public partial class Chart : ChartElement,IChart {
        private IChartCategories _categories;
        private IChartDatasets _datasets;
        private IChartLineSet _lineSet;
        private IChartTrendLines _trendLines;

        /// <summary>
        ///     ���������� XML-������������� ������ �� ��� �������
        /// </summary>
        /// <param name="chartConfig">������ �����</param>
        /// <returns>XML-������������� ������</returns>
        public XElement GenerateChartXml(IChartConfig chartConfig) {
            var render = CreateChartRender(chartConfig);
            var xml = render.GenerateChartXmlSource(chartConfig).GenerateChartXml(chartConfig);
            xml = render.RefactorChartXml(xml,chartConfig);
            return xml;
        }

        /// <summary>
        /// ���������, ����� ������� ����� ������� ��� ������� ������
        /// </summary>
        /// <param name="chartConfig"></param>
        /// <returns></returns>
        protected virtual IChartRender CreateChartRender(IChartConfig chartConfig) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     ������������ ���������
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
        /// ��������� ��� �������� �������� ��� ���������
        /// </summary>
        /// <returns></returns>
        protected virtual IChartCategories CreateCategoriesElement() {
            return new ChartCategories();
        }

        /// <summary>
        ///     ��������
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
        /// ������� ����� �������� ������
        /// </summary>
        /// <returns></returns>
        protected virtual IChartDatasets CreateDatasetsElement() {
            return new ChartDatasets();
        }

        /// <summary>
        /// ����� �������������� �����
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
        /// ������� ����� ������� ���������
        /// </summary>
        /// <returns></returns>
        protected virtual IChartLineSet CreateLinesetElement() {
            return new ChartLineSet();
        }

        /// <summary>
        /// ����� ����� ������
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
        /// �������� �������� ������� Trendline
        /// </summary>
        /// <returns></returns>
        public virtual IChartTrendLines CreateTrendlinesElement() {
            return new ChartTrendLines();
        }
    }
}