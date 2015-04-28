namespace Qorpent.Charts {
    /// <summary>
    /// Коллекция наборов данных
    /// </summary>
    public interface IChartDatasets:IChartRootElement<IChartDataset> {
        /// <summary>
        /// ИНициирует или возвращает датасет по умолчанию
        /// </summary>
        /// <returns></returns>
        IChartDataset EnsureDataset();
    }
}