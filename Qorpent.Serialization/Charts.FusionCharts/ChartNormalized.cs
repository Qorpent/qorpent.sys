using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     ����������� ������������� ��������������� ����� ������ ����������� �������
    /// </summary>
    public class ChartNormalized : IChartNormalized {
        private readonly IList<ChartAbstractScale> _scales = new List<ChartAbstractScale>();
        private readonly IDictionary<IChartElement, IDictionary<string, object>> _fixedAttributes = new Dictionary<IChartElement, IDictionary<string, object>>();
        private readonly IChart _snappedChart;
        /// <summary>
        ///     ������������� ������ ������������ ���������
        /// </summary>
        public IDictionary<IChartElement, IDictionary<string, object>> FixedAttributes {
            get { return _fixedAttributes; }
        }
        /// <summary>
        ///     ������������ ���� �������
        /// </summary>
        public IEnumerable<ChartAbstractScale> Scales {
            get { return _scales; }
        }
        /// <summary>
        ///     ������������� ���������������� �����
        /// </summary>
        public ChartNormalized() { }
        /// <summary>
        ///     �������������� ����� � ��������� ��������� ����� � ��� ����������� �������������
        /// </summary>
        /// <param name="chart">����</param>
        public ChartNormalized(IChart chart) {
            _snappedChart = chart;

            // �������� ������������� ����� ��������� � ��������
            chart.Datasets.Children.SelectMany(_ => _.Children).DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            // �������� ������������� ����������� � ��������
            chart.TrendLines.Children.DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            chart.Datasets.Children.DoForEach(_ => _.DoForEach(__ => AddFixedAttribute(_, __.Key, __.Value)));
            chart.DoForEach(_ => AddFixedAttribute(chart, _.Key, _.Value));
        }
        /// <summary>
        ///     ���������� ���������������� ���������� � ����������� �����
        /// </summary>
        /// <param name="chart">�������� ����</param>
        /// <returns>��������� �� ��������������� �������� ����</returns>
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
        ///     ���������� ����, � �������� ��������� �������������
        /// </summary>
        /// <returns>����, � �������� �������� ����</returns>
        public IChart GetSnappedChart() {
            return _snappedChart;
        }
        /// <summary>
        ///     ���������� ����� 
        /// </summary>
        /// <param name="abstractScale">����������� ������������� �����</param>
        public void AddScale(ChartAbstractScale abstractScale) {
            _scales.Add(abstractScale);
        }
        /// <summary>
        ///     ���������� ������������� ��������
        /// </summary>
        /// <param name="element">�������, � ������� ��������� �������</param>
        /// <param name="attribute">��� ��������</param>
        /// <param name="value">�������� ��������</param>
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
        ///     ���������� ������������ ���� ��������� �� ��������
        /// </summary>
        /// <param name="parent">������������ �������-��������</param>
        /// <returns>������� ������������ ���������</returns>
        public IDictionary<string, object> GetFixedAttributes(IChartElement parent) {
            return FixedAttributes.ContainsKey(parent) ? FixedAttributes[parent] : null;
        }
        /// <summary>
        ///     ���������� ������������ ���� ������������ ��������� �� �����
        /// </summary>
        /// <typeparam name="T">���������</typeparam>
        /// <param name="attribute">��� ��������</param>
        /// <returns>������������ ������������ ���������</returns>
        public IEnumerable<T> GetFixedAttributes<T>(string attribute) {
            return FixedAttributes.Where(_ => _.Value.ContainsKey(attribute)).Select(_ => (T)_.Value[attribute]);
        }
        /// <summary>
        ///     ���������� ������������ ���� ������������ ��������� �� ����� � ���� ��������
        /// </summary>
        /// <typeparam name="TP">��� ��������</typeparam>
        /// <typeparam name="T">��������� ��������</typeparam>
        /// <param name="attribute">��� ��������</param>
        /// <returns>������������ ������������ ���������</returns>
        public IEnumerable<T> GetFixedAttributes<TP, T>(string attribute) {
            return FixedAttributes.Where(
                _ => (_.Key is TP) && (_.Value.ContainsKey(attribute))
            ).Select(
                _ => (T)_.Value[attribute]
            );
        }
    }
}