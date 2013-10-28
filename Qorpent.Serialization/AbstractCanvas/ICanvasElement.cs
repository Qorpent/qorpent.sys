namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление элемента канвы
    /// </summary>
    public interface ICanvasElement {
        /// <summary>
        ///     Родительская канва
        /// </summary>
        ICanvas Parent { get; }
    }
}