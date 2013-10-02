namespace Qorpent.Charts {
    /// <summary>
    ///     Интерфейс рендера графиков
    /// </summary>
    public interface IChartRender : IChartXmlSource, IChartSource {
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <param name="config">Конфиг рендера чарта</param>
        /// <returns>Экземпляр данного класса</returns>
        IChartRender Initialize(IChart chart, IChartConfig config);
        /// <summary>
        ///     Отрендерить чарт по переданному представлению и конфигу
        /// </summary>
        /// <param name="chartRenderConfig">Представления конфига чарта</param>
        /// <returns>XML-представление отрендеренного чарта</returns>
        IChartRenderResult RenderChart(IChartConfig chartRenderConfig);
    }
}