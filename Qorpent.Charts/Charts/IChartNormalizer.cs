using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    ///     ��������� ������������� ������
    /// </summary>
    public interface IChartNormalizer : IScope {
        /// <summary>
        ///     ��� ������������
        /// </summary>
        int Code { get; }
        /// <summary>
        ///     ����������� ������������ �� ������ ��������������
        /// </summary>
        IEnumerable<int> Dependencies { get; }
        /// <summary>
        ///     ������� ������������ �����
        /// </summary>
        ChartNormalizerArea Area { get; }
        /// <summary>
        ///     ������������ �����
        /// </summary>
        /// <param name="chart">������������� ��������� �����</param>
        /// <param name="normalized">����������� ������������� ���������������� �����</param>
        /// <returns>��������� �� ����������� ������������� ���������������� �����</returns>
        IChartNormalized Normalize(IChart chart, IChartNormalized normalized);
        /// <summary>
        ///     ���������� ������� ����, ��� ���������� ������������� ���������������� ����� �������������� ��������
        /// </summary>
        /// <param name="normalized">������������� ���������������� �����</param>
        /// <returns>������� ����, ��� ���������� ������������� ���������������� ����� �������������� ��������</returns>
        bool IsSupported(IChartNormalized normalized);
        /// <summary>
        ///     ���������� ������� ����, ��� ���������� ������������� ����� �������������� ��������
        /// </summary>
        /// <param name="chart">������������� �����</param>
        /// <returns>������� ����, ��� ���������� ������������� ����� �������������� ��������</returns>
        bool IsSupported(IChart chart);
    }
}