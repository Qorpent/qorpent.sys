namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public interface IChartRenderResult {
        /// <summary>
        ///     Выходной формат рендера
        /// </summary>
        ChartRenderFormat RenderFormat { get; }
        /// <summary>
        ///     Отрендеренный график
        /// </summary>
        string Rendered { get; }
    }
}
