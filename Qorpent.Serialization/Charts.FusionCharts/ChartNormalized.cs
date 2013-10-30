using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Абстрактное представление нормалзованного чарта потипу разряжённого массива
    /// </summary>
    public class ChartNormalized : IChartNormalized {
        private readonly IList<ChartAbstractScale> _scales = new List<ChartAbstractScale>();
        private readonly IDictionary<IChartElement, IDictionary<string, object>> _fixedAttributes = new Dictionary<IChartElement, IDictionary<string, object>>();
        private readonly IChart _snappedChart;
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
        public ChartNormalized() { }
        /// <summary>
        ///     Инициализирует класс с переносом контекста чарта в его абстрактное представление
        /// </summary>
        /// <param name="chart">Чарт</param>
        public ChartNormalized(IChart chart) {
            _snappedChart = chart;

            // копируем представление сетов датасетов в контекст
            chart.Datasets.Children.SelectMany(_ => _.Children).DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            // копируем представление трендлайнов в контекст
            chart.TrendLines.Children.DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            chart.Datasets.Children.DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            chart.DoForEach(_ => AddFixedAttribute(chart, _.Key, _.Value));
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
        ///     Возвращает чарт, к которому привязано представление
        /// </summary>
        /// <returns>Чарт, к которому привязан чарт</returns>
        public IChart GetSnappedChart() {
            return _snappedChart;
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
        /// <summary>
        ///     Возвращает перечисление всех атрибутов по родителю
        /// </summary>
        /// <param name="parent">Родительский элемент-владалец</param>
        /// <returns>Словарь исправленных атрибутов</returns>
        public IDictionary<string, object> GetFixedAttributes(IChartElement parent) {
            return FixedAttributes.ContainsKey(parent) ? FixedAttributes[parent] : null;
        }
        /// <summary>
        ///     Возвращает перечисление всех исправленных атрибутов по имени
        /// </summary>
        /// <typeparam name="T">Типизация</typeparam>
        /// <param name="attribute">Имя атрибута</param>
        /// <returns>Перечисление исправленных атрибутов</returns>
        public IEnumerable<T> GetFixedAttributes<T>(string attribute) {
            return FixedAttributes.Where(_ => _.Value.ContainsKey(attribute)).Select(_ => (T)_.Value[attribute]);
        }
        /// <summary>
        ///     Возвращает перечисление всех исправленных атрибутов по имени и типу родителя
        /// </summary>
        /// <typeparam name="TP">Тип родителя</typeparam>
        /// <typeparam name="T">Типизация значения</typeparam>
        /// <param name="attribute">Имя атрибута</param>
        /// <returns>Перечисление исправленных атрибутов</returns>
        public IEnumerable<T> GetFixedAttributes<TP, T>(string attribute) {
            return FixedAttributes.Where(
                _ => (_.Key is TP) && (_.Value.ContainsKey(attribute))
            ).Select(
                _ => (T)_.Value[attribute]
            );
        }
    }
}