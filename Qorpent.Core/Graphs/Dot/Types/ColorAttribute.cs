using System;

namespace Qorpent.Graphs.Dot.Types {
    /// <summary>
    /// Описывает цвета для построения в Dot, поддерживает как режим SINGLECOLOR
    /// Так и ColorList
    /// </summary>
    public class ColorAttribute {
        private ColorAttribute() {}
        /// <summary>
        /// Создает атрибут из нативной строки
        /// </summary>
        /// <param name="nativeString"></param>
        /// <returns></returns>
        public static ColorAttribute Native(string nativeString) {
            if (string.IsNullOrWhiteSpace(nativeString)) throw new ArgumentException("empty color definition","nativeString");
            var result = new ColorAttribute { Mode = ColorAttributeType.Native, NativeString = nativeString };
            return result;
        }
        /// <summary>
        /// Строка на DOT
        /// </summary>
        public string NativeString { get; set; }

        /// <summary>
        /// Формирует описание единичного цвета
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ColorAttribute Single(Color color) {
            var result = new ColorAttribute {Mode = ColorAttributeType.Single, Color = color};
            return result;
        }
        /// <summary>
        /// Формирует описание множественного цвета
        /// </summary>
        public static ColorAttribute Multiple(ColorList colorList)
        {
            if (null == colorList || 0 == colorList.Count) throw new ArgumentException("items must be given", "colorList");
            var result = new ColorAttribute { Mode = ColorAttributeType.Multiple, ColorList = colorList };
            return result;
        }
        /// <summary>
        /// Список цветов
        /// </summary>
        public ColorList ColorList { get; private set; }

        /// <summary>
        /// Базовый цвет
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Режим представления света
        /// </summary>
        public ColorAttributeType Mode { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            if (Mode == ColorAttributeType.Native) {
                return NativeString;
            }
            if (Mode == ColorAttributeType.Single) {
                return Color.ToString();
            }
            return ColorList.ToString();
        }

        /// <summary>
        /// Строки автоматически конвертируются в цвет нативного типа
        /// </summary>
        /// <param name="nativeString"></param>
        /// <returns></returns>
        public static implicit operator ColorAttribute(string nativeString) {
            return Native(nativeString);
        }
        /// <summary>
        /// Строки автоматически конвертируются в цвет нативного типа
        /// </summary>
        /// <returns></returns>
        public static implicit operator ColorAttribute(ColorList list) {
            return Multiple(list);
        }


    }
}