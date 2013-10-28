using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qorpent.Graphs.Dot.Types {




    /// <summary>
    /// Описание конца стрелки
    /// </summary>
    public class Arrow {
        private IList<ArrowType> _types;
        /// <summary>
        /// Множественный тип
        /// </summary>
        public bool IsMultiple {
            get { return null != _types && 0 != _types.Count; }
        }
        /// <summary>
        /// Базовый тип
        /// </summary>
        public ArrowType MainType { get; set; }
        /// <summary>
        /// Список типов
        /// </summary>
        public IList<ArrowType> Types {
            get { return _types ?? (_types= new List<ArrowType>()); }
        }

        /// <summary>
        /// Тип стрелок
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static implicit operator Arrow(ArrowType type) {
            return new Arrow {MainType = type};
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Arrow operator +(Arrow a,ArrowType type)
        {
            if (!a.IsMultiple) {
                a.Types.Add(a.MainType);
            }
            a.Types.Add(type);
            return a;
        }
       

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            if (IsMultiple) {
                return string.Join("", Types.Select(GetSingleTypeString));
            }
            return GetSingleTypeString(MainType);
        }

        /// <summary>
        /// Конвертирует описатель типа стрелки в DOT
        /// </summary>
        /// <param name="arrow"></param>
        /// <returns></returns>
        public static string GetSingleTypeString(ArrowType arrow) {
            var sb = new StringBuilder();
            if (0!=(arrow & ArrowType.SideAble)) {
                if (arrow.HasFlag(ArrowType.Left)) {
                    sb.Append('l');
                }else if (arrow.HasFlag(ArrowType.Right)) {
                    sb.Append('r');
                }
            }
            if (0!=(arrow & ArrowType.EmptyAble)) {
                if (arrow.HasFlag(ArrowType.Empty)) {
                    sb.Append('o');
                }
            }
            if (arrow.HasFlag(ArrowType.Inv)) {
                sb.Append("Inv");
            }
            else if (arrow.HasFlag(ArrowType.Dot)) {
                sb.Append("dot");
            }
            else if (arrow.HasFlag(ArrowType.Tee))
            {
                sb.Append("tee");
            }
            else if (arrow.HasFlag(ArrowType.Vee))
            {
                sb.Append("vee");
            }
            else if (arrow.HasFlag(ArrowType.Inv))
            {
                sb.Append("inv");
            }
            else if (arrow.HasFlag(ArrowType.Diamond))
            {
                sb.Append("diamond");
            }
            else if (arrow.HasFlag(ArrowType.Curve))
            {
                sb.Append("curve");
            }
            else if (arrow.HasFlag(ArrowType.Box))
            {
                sb.Append("box");
            }
            return sb.ToString();
        }
    }
}