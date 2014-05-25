using System;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// Обертка графика для соответствия FusionChart
    /// </summary>
    public class FusionChartWrapper  {
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
            _config = config??_config;
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
    }
}