namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление оси <see cref="ICanvas"/>
    /// </summary>
    public interface ICanvasAxis : ICanvasElement {
        /// <summary>
        ///     Начальное значение оси
        /// </summary>
        double BeginValue { get; }
        /// <summary>
        ///     Конечное значение оси
        /// </summary>
        double EndValue { get; }
        /// <summary>
        ///     Установка родительской канвы
        /// </summary>
        /// <param name="canvas">Родительский экземпляр <see cref="ICanvas"/></param>
        /// <returns>Замыкание на <see cref="ICanvasAxis"/></returns>
        ICanvasAxis SetCanvas(ICanvas canvas);
    }
}