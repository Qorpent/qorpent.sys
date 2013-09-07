using System.Collections.Generic;
using Qorpent.Mvc;

namespace Qorpent.Serialization {

    

    /// <summary>
    /// Параметры конвертации в DOT
    /// </summary>
    public class GraphOptions {
        /// <summary>
        /// Диалект - DOT
        /// </summary>
        public const string DOTDIALECT= "dot";
        /// <summary>
        /// Алгоритм - DOT
        /// </summary>
        public const string DOTAGORITHM = "dot";
        /// <summary>
        /// Цель - SVG
        /// </summary>
        public const string SVGFORMAT = "svg";
        /// <summary>
        /// Цель - PNG
        /// </summary>
        public const string PNGFORMAT = "png";
        /// <summary>
        /// Цель - SVG
        /// </summary>
        public const string PNGTARGET = "png";

        /// <summary>
        /// 
        /// </summary>
        public GraphOptions() {
            OverrideGraphAttributes = new Dictionary<string, object>();
            Format = SVGFORMAT;
            Algorithm = "dot";
            Dialect = DOTDIALECT;
            Tune = true;
        }
        /// <summary>
        /// Позволяет указать параметры при перекрытии
        /// </summary>
        public IDictionary<string, object> OverrideGraphAttributes { get; private set; }

        /// <summary>
        /// Указывает целевой формат
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Признак того, что граф должен быть нормализован перед генерацией
        /// </summary>
        public bool Tune { get; set; }
        /// <summary>
        /// Указывает алгоритм генерации
        /// </summary>
        public string Algorithm { get; set; }
        /// <summary>
        /// Указывает диалект
        /// </summary>
        public string Dialect { get; set; }
        /// <summary>
        /// Дополнительный объект - источник информации
        /// </summary>
        public object CustomData { get; set; }
        /// <summary>
        /// Внешний контекст вызова графа
        /// </summary>
        public IMvcContext Context { get; set; }
    }
}