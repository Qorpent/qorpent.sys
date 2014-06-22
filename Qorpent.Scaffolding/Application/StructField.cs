namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    public class StructField {
        /// <summary>
        /// Тип поля
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Читаемое имя или комментарий
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Значение по-умолчанию
        /// </summary>
        public string DefaultValue { get; set; }
    }
}