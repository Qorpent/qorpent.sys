namespace Qorpent.Charts {
    /// <summary>
    ///     абстрактное представление шкалы графика
    /// </summary>
    public class ChartAbstractScale {
        /// <summary>
        ///     Позиция шкалы
        /// </summary>
        public ChartAbstractScaleType ScaleType { get; set; }
        /// <summary>
        ///     Минимальное значение для шкалы
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        ///     Максимальное значение для шкалы
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        ///     Кол-во дивлайнов для шкалы
        /// </summary>
        public int NumDivLines { get; set; }
    }
}