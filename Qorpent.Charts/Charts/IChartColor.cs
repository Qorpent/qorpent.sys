namespace Qorpent.Charts {
    /// <summary>
    ///     Представление цвета элемента
    /// </summary>
    public interface IChartColor {
        /// <summary>
        ///     Элемент-владелец
        /// </summary>
        IChartElement Owner { get; }
        /// <summary>
        /// 
        /// </summary>
        int ColorCode { get; }
    }
}
