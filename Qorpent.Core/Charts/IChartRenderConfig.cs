namespace Qorpent.Charts {
    /// <summary>
    ///     Конфиг чарт рендера
    /// </summary>
    public interface IChartRenderConfig {
        /// <summary>
        ///     Выходной формат рендера
        /// </summary>
        ChartRenderFormat RenderFormat { get; set; }
    }
}
