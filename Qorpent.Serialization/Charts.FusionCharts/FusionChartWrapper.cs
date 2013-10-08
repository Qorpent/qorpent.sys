using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// Обертка графика для соответствия FusionChart
    /// </summary>
    public partial class FusionChartWrapper  {
        private IChartElement _element;
        private IChartConfig _config;
        private string _elementName;
        private FusionChartElementType _eltype;
        private FusionChartType _charttype;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="config"></param>
        /// <exception cref="NotImplementedException"></exception>
        public FusionChartWrapper(IChartElement element, IChartConfig config) {
            _element = element;
            _config = config;
            _elementName = determineName(_element);
            _eltype = _elementName.To<FusionChartElementType>();
            _charttype = config.Type.To<FusionChartType>();
        }

        private string determineName(IChartElement element) {
            if (element is IChart) return "chart";
            if (element is IChartSet) return "set";
            if (element is IChartTrendLine) return "line";
            throw new Exception("unknown type " + element.GetType().Name);
        }

        /// <summary>
        ///Исходный чарт
        /// </summary>
        public IChartElement Element {
            get { return _element; }
        }
        /// <summary>
        /// Заголовок чарта
        /// </summary>
        public string Caption {
            get { return _element.Get<string>(FusionChartApi.Chart_Caption); }
            set { _element.Set(FusionChartApi.Chart_Caption,value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement GetXmlElement() {
            var result = new XElement(_elementName);
            foreach (var attr in FusionChartApi.FindAttributes(_charttype, _eltype)) {
                if (_element.ContainsKey(attr.Name)) {
                    result.SetAttributeValue(attr.Name,_element.Get<string>(attr.Name));
                }
            }
            return result;
        }
        /// <summary>
        /// Добавляет элемент в первый датасет
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IChartSet AddSet(decimal value) {
            return AddSet(null, value);
        }

        /// <summary>
        /// Добавляет элемент в первый датасет
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IChartSet AddSet(string label, decimal value) {
            var dataset = ((IChart) _element).Datasets.EnsureDataset();
            var el = new ChartSet();
            if (!string.IsNullOrWhiteSpace(label)) {
                el.Set(FusionChartApi.Set_Label, label);
                el.Set(FusionChartApi.Set_Value, value);
            }
            dataset.Add(el);
            return el;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sets"></param>
        public void AddSets(object sets) {
            foreach (var s in sets.ToDict()) {
                AddSet(s.Key, s.Value.ToDecimal());
            }
        }
        /// <summary>
        ///     Добавление сформированного датасета
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="dataset">Перечисление датасетов</param>
        public void AddDataset(IChart chart, IChartDataset dataset) {
            chart.Datasets.Add(dataset);
        }
        /// <summary>
        ///     Добавление перечисления датасета
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="datasets">Перечисление датасетов</param>
        public void AddDatasets(IChart chart, IEnumerable<IChartDataset> datasets) {
            foreach (var dataset in datasets) {
                AddDataset(chart, dataset);
            }
        }
        /// <summary>
        ///     Добавление линии тренда
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="value">Значение, по которому проходит линия тренда</param>
        public void AddTrendLine(IChart chart, double value) {
            chart.TrendLines.Add(new ChartLine {StartValue = value});
        }
        /// <summary>
        ///     Добавление линии тренда
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="trendLine">Представление линии тренда</param>
        public void AddTrendLine(IChart chart, IChartTrendLine trendLine) {
            chart.TrendLines.Add(trendLine);
        }
    }
}