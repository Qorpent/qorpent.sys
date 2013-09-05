using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Dot.Colors {
    /// <summary>
    /// Тип определения цвета
    /// </summary>
    public enum ColorAttributeType {
        /// <summary>
        /// Одиночный цвет
        /// </summary>
        Single,

        /// <summary>
        /// Градиент
        /// </summary>
        Multiple,
        /// <summary>
        /// Явно заданный Dot строкой
        /// </summary>
        Native,
    }

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
            if(null==color)throw new ArgumentNullException("color");
            var result = new ColorAttribute {Mode = ColorAttributeType.Single, Color = color};
            return result;
        }
        /// <summary>
        /// Формирует описание множественного цвета
        /// </summary>
        public static ColorAttribute Multiple(ColorList colorList)
        {
            if (null == colorList || 0 == colorList.Count) throw new ArgumentException("items must be given", "colorList");
            var result = new ColorAttribute { Mode = ColorAttributeType.Single, ColorList = colorList };
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
    }

    /// <summary>
    /// Описывает цвет
    /// </summary>
    public class Color {
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

    }
    /// <summary>
    /// Набор цветов для градиентов
    /// </summary>
    public class ColorList:List<ColorListItem> {
        private bool _wait_weight;

        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <returns></returns>
        public static ColorList operator +(ColorList list,ColorListItem item)
        {
           list.Add(item);
            return list;
        }

        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <returns></returns>
        public static ColorList operator +(ColorList list, Color color)
        {
            list.Add(new ColorListItem{Color = color});
            list._wait_weight = true;
            return list;
        }
        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <returns></returns>
        public static ColorList operator +(ColorList list, decimal weight)
        {
            if(0==list.Count)throw new InvalidOperationException("cannot assign weight if no items existed");
            if (!list._wait_weight) throw new InvalidOperationException("invalid twice or weight-first color list expression");
            list._wait_weight = false;
            list[list.Count - 1].Weight = weight;
            return list;
        }
        /// <summary>
        /// ФОрмирует DOT-строку
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Join(":", this.Select(_ => _.ToString()));
        }
    }

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
            return Color + ";" + Weight;
        }
    }
}