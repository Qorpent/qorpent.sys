using System;
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
            get { return _element.Get<string>(Api.Chart_Caption); }
            set { _element.Set(Api.Chart_Caption,value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement GetXmlElement() {
            var result = new XElement(_elementName);
            foreach (var attr in Api.FindAttributes(_charttype, _eltype)) {
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
                el.Set(Api.Set_Label, label);
                el.Set(Api.Set_Value, value);
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
    }
}