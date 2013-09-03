using System.Collections.Generic;

namespace Qorpent.BSharp.Builder {
    /// <summary>
    ///     Класс описывает цели компилятора
    /// </summary>
    public class BSharpBuilderTargets {
        /// <summary>
        ///     Перечисление нэймспэйсов для сборки
        /// </summary>
        public IDictionary<string, BSharpBuilderTargetType> Namespaces { get; set; }
        /// <summary>
        ///     Перечисление целевых классов для копиляции
        /// </summary>
        public IDictionary<string, BSharpBuilderTargetType> Classes { get; set; }
        /// <summary>
        ///     Перечисление путей, по которым нужно брать
        ///     определённый файл или делать рекурсивный
        ///     поиск
        /// </summary>
        public IDictionary<string, BSharpBuilderTargetType> Paths { get; set; }
        /// <summary>
        ///     
        /// </summary>
        public BSharpBuilderTargets() {
            Namespaces = new Dictionary<string, BSharpBuilderTargetType>();
            Classes = new Dictionary<string, BSharpBuilderTargetType>();
            Paths = new Dictionary<string, BSharpBuilderTargetType>();
        }
       
    }
}
