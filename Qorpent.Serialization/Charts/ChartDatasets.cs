namespace Qorpent.Charts {
    /// <summary>
    /// �������������� ����� ���������
    /// </summary>
    public class ChartDatasets : ChartElementList<IChart,IChartDataset>,IChartDatasets
    {
        /// <summary>
        /// ���������� ��� ���������� ������� �� ���������
        /// </summary>
        /// <returns></returns>
        public IChartDataset EnsureDataset() {
            if (0 != Children.Count) return Children[0];
            var ds = new ChartDataset();
            Add(ds);
            return ds;
        }
    }
}