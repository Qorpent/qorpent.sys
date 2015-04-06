using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Graphs.Dot.Types {
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
        public static ColorList operator +(ColorList list, double weight) {
            return list + Convert.ToDecimal(weight);
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

        /// <summary>
        /// Конвертирует отдельные цвета в атрибуты типа Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorList(Color color) {
            var result = new ColorList {color};
            return result;
        }
        /// <summary>
        /// Конвертирует отдельные цвета в атрибуты типа Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorList(ColorListItem color)
        {
            var result = new ColorList { color };
            return result;
        }
    }
}