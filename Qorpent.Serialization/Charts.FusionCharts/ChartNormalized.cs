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
        public ChartNormalized() {
            _scales = new List<ChartAbstractScale>();
            _fixedAttributes = new Dictionary<IChartElement, IDictionary<string, object>>();
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
    }
}