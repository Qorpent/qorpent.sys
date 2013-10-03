namespace Qorpent.Charts {
    /// <summary>
    /// Имплементирует набор датасетов
    /// </summary>
    public class ChartDatasets : ChartElementList<IChart,IChartDataset>,IChartDatasets
    {
        /// <summary>
        /// ИНициирует или возвращает датасет по умолчанию
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