namespace Qorpent.Charts {
    /// <summary>
    ///     ��������� ������������� ������
    /// </summary>
    public interface IChartNormalizer {
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