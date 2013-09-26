namespace Qorpent.Charts {
    /// <summary>
    ///     Интерфейс рендера графиков
    /// </summary>
    public interface IChartRender {
        /// <summary>
        ///     Прокатывает операцию рендеринга
        /// </summary>
        /// <param name="chart">Представление графика</param>
        /// <returns>Результат работы рендера</returns>
        IChartRenderResult Render(IChart chart);
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chartRenderConfig">Конфиг рендера чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        IChartRender Initialize(IChartRenderConfig chartRenderConfig);
    }
}