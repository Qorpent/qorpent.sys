using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.AbstractCanvas {
    /// <summary>
    ///     Представление канвы
    /// </summary>
    public class Canvas : ConfigBase, ICanvas {
        private KeyValuePair<double, double> _x;
        private KeyValuePair<double, double> _y;
        /// <summary>
        ///     Внутреннее представление перечисления точек, относящихся к канве
        /// </summary>
        private readonly IList<ICanvasPrimitive> _primitives = new List<ICanvasPrimitive>();
        /// <summary>
        ///     Перечисление примитивов, относящихся к канве
        /// </summary>
        public IEnumerable<ICanvasPrimitive> Primitives {
            get { return _primitives; }
        }
        /// <summary>
        ///     Представление оси X
        /// </summary>
        public ICanvasAxis X { get; private set; }
        /// <summary>
        ///     Представление оси Y
        /// </summary>
        public ICanvasAxis Y { get; private set; }
        /// <summary>
        ///     Длина
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        ///     Ширина
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        ///     Возвращает величину на пиксель
        /// </summary>
        public double ValuePerPixes {
            get { return (Width.Minimal(Height))/((_x.Value - _x.Key).Minimal(_y.Value - _y.Key)); }
        }
        /// <summary>
        ///     Представление канвы
        /// </summary>
        /// <param name="xFrom">Начало оси X</param>
        /// <param name="xTo">Конец оси X</param>
        /// <param name="yFrom">Начало оси Y</param>
        /// <param name="yTo">Конец оси Y</param>
        public Canvas(double xFrom, double xTo, double yFrom, double yTo) {
            if ((xTo < xFrom) || (yTo < yFrom)) {
                throw new Exception("Incorrect X or Y begin and end values");
            }

            _x = new KeyValuePair<double, double>(xFrom, xTo);
            _y = new KeyValuePair<double, double>(yFrom, yTo);

            X = new CanvasAxis(xFrom, xTo).SetCanvas(this);
            Y = new CanvasAxis(yFrom, yTo).SetCanvas(this);

            Scale(
                (xTo.RoundUp(0) - xFrom.RoundDown(0)).ToInt(),
                (yTo.RoundUp(0) - yFrom.RoundDown(0)).ToInt()
            );
        }
        /// <summary>
        ///     Скалирование канвы
        /// </summary>
        /// <param name="width">Длина</param>
        /// <param name="height">Ширина</param>
        /// <returns>Замыкание на шкалированный график</returns>
        public ICanvas Scale(int width, int height) {
            Width = width;
            Height = height;
            return this;
        }
        /// <summary>
        ///     Установка точки
        /// </summary>
        /// <param name="x">Позиция по X</param>
        /// <param name="y">Позиция по Y</param>
        /// <returns></returns>
        public ICanvasPoint SetPoint(double x, double y) {
            var point = new CanvasPoint(x, y);
            _primitives.Add(point);
            return point;
        }
        /// <summary>
        ///     Возвращает перечисление всех, элементов, которые неподалеку указанного элемента
        /// </summary>
        /// <param name="canvasPrimitive">Представление элемента</param>
        /// <param name="radius">Радиус поиска (в пикселях)</param>
        /// <returns>Перечисление элементов, находящихся неподалеку</returns>
        public IEnumerable<ICanvasPrimitive> Nearby(ICanvasPrimitive canvasPrimitive, int radius) {
            return _primitives.Where(
                _ => (
                    Math.Pow((_.X - canvasPrimitive.X) * ValuePerPixes, 2) + Math.Pow((_.Y - canvasPrimitive.Y) * ValuePerPixes, 2) <= (Math.Pow(radius, 2))
                ) && (
                    !_.Equals(canvasPrimitive)
                )
            );
        }
    }
}
