namespace Qorpent.Charts {
    /// <summary>
    ///     Интерфейс нормалайзера графиков
    /// </summary>
    public interface IСhartNormalizer {
        /// <summary>
        ///     Произвести нормализацию чарта
        /// </summary>
        /// <param name="chart">Чарт</param>
        /// <returns>Нормализованный чарт</returns>
        IChart Normalize(IChart chart);
        /// <summary>
        ///     Инициализация нормалайзера
        /// </summary>
        /// <param name="chartConfig">Конфиг графика</param>
        /// <returns>Настроенный экземпляр <see cref="IСhartNormalizer"/></returns>
        IСhartNormalizer Initialize(IChartConfig chartConfig);
    }
}
