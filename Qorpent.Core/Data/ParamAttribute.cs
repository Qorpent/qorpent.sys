using System;

namespace Qorpent.Data {
    /// <summary>
    /// Помечает в классе свойства для мапинга в параметры SQL
    /// </summary>
    public class ParamAttribute:Attribute {
        /// <summary>
        /// Заместитель имени параметра
        /// </summary>
        public string Name { get; set; }
    }
}