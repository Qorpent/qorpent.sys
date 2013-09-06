using System;
using System.Linq;

namespace Qorpent.Dot.Colors {
    /// <summary>
    /// Описывает цвет
    /// </summary>
    public class Color {
        private static Color _red;
        /// <summary>
        /// Красный
        /// </summary>
        public static Color Red
        {
            get { return _red ?? (_red = new Color("red")); }
        }


        private static Color _green;
        private static Color _blue;

        private Color(string native) {
            this.NativeString = native;
        }
        /// <summary>
        /// Определение цвета в DOT
        /// </summary>
        public string NativeString { get;private set; }

        /// <summary>
        /// Конвертирует отдельные цвета в атрибуты типа Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorAttribute(Color color) {
            return ColorAttribute.Single(color);
        }

        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ColorListItem operator +(Color color, decimal weight) {
            return new ColorListItem{Color = color,Weight = weight};
        }
        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ColorListItem operator +(Color color, double weight)
        {
            return new ColorListItem { Color = color, Weight = Convert.ToDecimal(weight) };
        }
        /// <summary>
        /// Формирует RGB цвет
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color RGB(byte r, byte g, byte b, byte a = 0) {
            var str = "#" + r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
            if (0 != a) {
                str += a.ToString("x2");
            }
            return new Color(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static Color Create(string def) {
            if (def.All(_ => (_ >= '0' && _ <= '9') || (_ >= 'A' && _ <= 'F') || (_ >= 'a' && _ <= 'f'))) {
                return new Color("#"+def);
            }

            return new Color(def);
        }

        
        /// <summary>
        /// Зеленый
        /// </summary>
        public static Color Green
        {
            get { return _green ?? (_green = new Color("green")); }
        }
        /// <summary>
        /// Синий
        /// </summary>
        public static Color Blue
        {
            get { return _blue ?? (_blue = new Color("blue")); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return NativeString;
        }
    }
}