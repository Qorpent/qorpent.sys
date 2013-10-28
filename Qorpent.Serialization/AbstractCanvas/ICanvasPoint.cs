namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление точки <see cref="ICanvas"/>
    /// </summary>
    public interface ICanvasPoint : ICanvasPrimitive {
        /// <summary>
        ///     Установка родительской канвы
        /// </summary>
        /// <param name="canvas">Родительский экземпляр <see cref="ICanvas"/></param>
        /// <returns>Замыкание на <see cref="ICanvasPoint"/></returns>
        ICanvasPoint SetCanvas(ICanvas canvas);
    }
}