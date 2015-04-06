using System.Globalization;

namespace Qorpent.Graphs.Dot.Types {
    /// <summary>
    /// Описывает элемент списка цветов
    /// </summary>
    public class ColorListItem {
        /// <summary>
        /// Цвет
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Формирует строку с элементом градиента
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            if (0 == Weight) {
                return Color.ToString();
            }
            return Color + ";" + Weight.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Конвертирует отдельные цвета в атрибуты типа Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorListItem(Color color)
        {
            var result = new ColorListItem {Color = color};
            return result;
        }

        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <returns></returns>
        public static ColorList operator +(ColorListItem item, Color color) {
            return new ColorList() + item + color;
        }
    }
}