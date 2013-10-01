namespace Qorpent.Charts {
    /// <summary>
    ///     Атрибут элемента чарта
    /// </summary>
    public class ChartAttribute : IChartAttribute {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public IChartElement ParentElement { get; set; }
        /// <summary>
        ///     Имя атрибута
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Значение атрибута
        /// </summary>
        public string Value { get; set; }
    }
}
