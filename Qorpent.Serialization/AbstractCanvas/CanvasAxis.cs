namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление оси <see cref="ICanvas"/>
    /// </summary>
    public class CanvasAxis : ICanvasAxis {
        /// <summary>
        ///     Родительская канва
        /// </summary>
        public ICanvas Parent { get; private set; }
        /// <summary>
        ///     Начальное значение оси
        /// </summary>
        public double BeginValue { get; private set; }
        /// <summary>
        ///     Конечное значение оси
        /// </summary>
        public double EndValue { get; private set; }
        /// <summary>
        ///     Представление оси <see cref="ICanvas"/>
        /// </summary>
        /// <param name="begin">Начало оси</param>
        /// <param name="end">Конец оси</param>
        public CanvasAxis(double begin, double end) {
            BeginValue = begin;
            EndValue = end;
        }
        /// <summary>
        ///     Установка родительской канвы
        /// </summary>
        /// <param name="canvas">Родительский экземпляр <see cref="ICanvas"/></param>
        /// <returns>Замыкание на <see cref="ICanvasAxis"/></returns>
        public ICanvasAxis SetCanvas(ICanvas canvas) {
            Parent = canvas;
            return this;
        }
    }
}