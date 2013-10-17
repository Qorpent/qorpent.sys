namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление примитива канвы
    /// </summary>
    public interface ICanvasPrimitive : ICanvasElement {
        /// <summary>
        ///     Позиция по оси X
        /// </summary>
        double X { get; }
        /// <summary>
        ///     Позиция по оси Y
        /// </summary>
        double Y { get; }
    }
}