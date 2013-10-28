namespace Qorpent.Charts {
    /// <summary>
    ///     ����������� ������������� ����� �������
    /// </summary>
    public class ChartAbstractScale {
        /// <summary>
        ///     ������� �����
        /// </summary>
        public ChartAbstractScaleType ScaleType { get; set; }
        /// <summary>
        ///     ����������� �������� ��� �����
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        ///     ������������ �������� ��� �����
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        ///     ���-�� ��������� ��� �����
        /// </summary>
        public int NumDivLines { get; set; }
    }
}