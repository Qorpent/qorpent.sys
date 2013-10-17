namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление точки <see cref="ICanvas"/>
    /// </summary>
    public class CanvasPoint : ICanvasPoint {
        /// <summary>
        ///     Родительская канва
        /// </summary>
        public ICanvas Parent { get; private set; }
        /// <summary>
        ///     Позиция по оси X
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        ///     Позиция по оси Y
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        ///     Объект, к которому относится данное значение
        /// </summary>
        public object Owner { get; private set; }
        /// <summary>
        ///     Представление точки <see cref="ICanvas"/>
        /// </summary>
        /// <param name="x">Позиция по X</param>
        /// <param name="y">Позиция по Y</param>
        public CanvasPoint(double x, double y) {
            X = x;
            Y = y;
        }
        /// <summary>
        ///     Установка родительской канвы
        /// </summary>
        /// <param name="canvas">Родительский экземпляр <see cref="ICanvas"/></param>
        /// <returns>Замыкание на <see cref="ICanvasPoint"/></returns>
        public ICanvasPoint SetCanvas(ICanvas canvas) {
            Parent = canvas;
            return this;
        }
        /// <summary>
        ///     Устанавливает объект-родитель
        /// </summary>
        /// <param name="owner">Объект-родитель</param>
        /// <returns>Замыкание</returns>
        public ICanvasPrimitive SetOwner(object owner) {
            Owner = owner;
            return this;
        }
    }
}