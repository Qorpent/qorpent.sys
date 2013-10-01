namespace Qorpent.Charts {
    /// <summary>
    /// Коллекция наборов данных
    /// </summary>
    public interface IChartDatasets:IChartElementList<IChartDataset> {
        /// <summary>
        /// ИНициирует или возвращает датасет по умолчанию
        /// </summary>
        /// <returns></returns>
        IChartDataset EnsureDataset();
    }
}