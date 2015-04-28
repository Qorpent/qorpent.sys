namespace Qorpent.Charts {
    /// <summary>
    ///     Атрибут элемента чарта
    /// </summary>
    public interface IChartAttribute {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        IChartElement ParentElement { get; }
        /// <summary>
        ///     Имя атрибута
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Значение атрибута
        /// </summary>
        string Value { get; }
    }
}