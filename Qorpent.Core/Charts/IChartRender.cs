namespace Qorpent.Charts {
    /// <summary>
    ///     Интерфейс рендера графиков
    /// </summary>
    public interface IChartRender : IChartXmlSource, IChartSource {
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        IChartRender Initialize(IChart chart, IChartConfig chartConfig);
    }
}