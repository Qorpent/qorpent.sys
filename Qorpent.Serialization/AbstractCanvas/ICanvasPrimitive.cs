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
        /// <summary>
        ///     Объект, к которому относится данное значение
        /// </summary>
        object Owner { get; }
        /// <summary>
        ///     Устанавливает объект-родитель
        /// </summary>
        /// <param name="owner">Объект-родитель</param>
        /// <returns>Замыкание</returns>
        ICanvasPrimitive SetOwner(object owner);
    }
}