namespace Qorpent.Charts {
    /// <summary>
    ///     Представление чарт-билдера
    /// </summary>
    public interface IChartBuilder {
        /// <summary>
        ///     Возвращает собарнный чарт
        /// </summary>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChart"/></returns>
        IChart GenerateChart();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IChartBuilder AddCategory(IChartElement category);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IChartBuilder AddDataset(IChartElement dataset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineset"></param>
        /// <returns></returns>
        IChartBuilder AddLineset(IChartElement lineset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trendline"></param>
        /// <returns></returns>
        IChartBuilder AddTrendLine(IChartElement trendline);
    }
}
