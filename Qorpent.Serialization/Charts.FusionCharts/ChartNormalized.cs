using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class ChartNormalized : IChartNormalized {
        private readonly IList<ChartAbstractScale> _scales;
        private readonly IDictionary<IChartElement, IDictionary<string, object>> _fixedAttributes;
        /// <summary>
        ///     Представление списка исправленных атрибутов
        /// </summary>
        public IDictionary<IChartElement, IDictionary<string, object>> FixedAttributes {
            get { return _fixedAttributes; }
        }
        /// <summary>
        ///     Перечисление шкал графика
        /// </summary>
        public IEnumerable<ChartAbstractScale> Scales {
            get { return _scales; }
        }
        /// <summary>
        ///     Представление нормализованного чарта
        /// </summary>
        public ChartNormalized() {
            _scales = new List<ChartAbstractScale>();
            _fixedAttributes = new Dictionary<IChartElement, IDictionary<string, object>>();
        }
        /// <summary>
        ///     Применение нолрмализованных параметров к переданному чурта
        /// </summary>
        /// <param name="chart">Исходный чарт</param>
        /// <returns>Замыкание на нормализованный исходный чарт</returns>
        public IChart Apply(IChart chart) {
            var xAxis = Scales.FirstOrDefault(_ => _.ScaleType == ChartAbstractScaleType.Y);
            if (xAxis != null) {
                chart.SetYAxisMinValue(xAxis.MinValue);
                chart.SetYAxisMaxValue(xAxis.MaxValue);
                chart.SetNumDivLines(xAxis.NumDivLines);
            }

            foreach (var element in _fixedAttributes) {
                foreach (var attribute in element.Value) {
                    element.Key.Set(attribute.Key, attribute.Value);
                }
            }

            return chart;
        }
        /// <summary>
        ///     Добавление шкалы 
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        public void AddScale(ChartAbstractScale abstractScale) {
            _scales.Add(abstractScale);
        }
        /// <summary>
        ///     Добавление исправленного атрибута
        /// </summary>
        /// <param name="element">Элемент, с которым соотнести атрибут</param>
        /// <param name="attribute">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public void AddFixedAttribute(IChartElement element, string attribute, object value) {
            if (!_fixedAttributes.ContainsKey(element)) {
                _fixedAttributes.Add(element, new Dictionary<string, object>());
            }

            if (_fixedAttributes[element].ContainsKey(attribute)) {
                _fixedAttributes[element][attribute] = value;
            } else {
                _fixedAttributes[element].Add(attribute, value);
            }
        }
    }
}