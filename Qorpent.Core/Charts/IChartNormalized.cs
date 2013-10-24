using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public interface IChartNormalized {
        /// <summary>
        ///     ������������� ������ ������������ ���������
        /// </summary>
        IDictionary<IChartElement, IDictionary<string, object>> FixedAttributes { get; }
        /// <summary>
        ///     ������������ ���� �������
        /// </summary>
        IEnumerable<ChartAbstractScale> Scales { get; }
        /// <summary>
        ///     ���������� ���������������� ���������� � ����������� �����
        /// </summary>
        /// <param name="chart">�������� ����</param>
        /// <returns>��������� �� ��������������� �������� ����</returns>
        IChart Apply(IChart chart);
        /// <summary>
        ///     ���������� ����� 
        /// </summary>
        /// <param name="abstractScale">����������� ������������� �����</param>
        void AddScale(ChartAbstractScale abstractScale);
        /// <summary>
        ///     ���������� ������������� ��������
        /// </summary>
        /// <param name="element">�������, � ������� ��������� �������</param>
        /// <param name="attribute">��� ��������</param>
        /// <param name="value">�������� ��������</param>
        void AddFixedAttribute(IChartElement element, string attribute, object value);
    }
}