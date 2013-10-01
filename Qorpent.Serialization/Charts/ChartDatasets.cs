namespace Qorpent.Charts {
    /// <summary>
    /// Имплементирует набор датасетов
    /// </summary>
    public class ChartDatasets : ChartElementList<IChartDataset>,IChartDatasets
    {
        /// <summary>
        /// ИНициирует или возвращает датасет по умолчанию
        /// </summary>
        /// <returns></returns>
        public IChartDataset EnsureDataset() {
            if (0 != RealList.Count) return RealList[0];
            var ds = new ChartDataset();
            ds.SetParent(this);
            Children.Add(ds);
            RealList.Add(ds);
            return ds;
        }
    }
}