using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    ///     »нтерфейс нормализатора чартов
    /// </summary>
    public interface IChartNormalizer : IScope {
        /// <summary>
        ///      од нормалайзера
        /// </summary>
        int Code { get; }
        /// <summary>
        ///     «ависимости нормалайзера от других нормализаторов
        /// </summary>
        IEnumerable<int> Dependencies { get; }
        /// <summary>
        ///     ќбласть нормализации чарта
        /// </summary>
        ChartNormalizerArea Area { get; }
        /// <summary>
        ///     Ќормализаци€ чарта
        /// </summary>
        /// <param name="chart">ѕредставление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>«амыкание на абстрактное представление нормализованного чарта</returns>
        IChartNormalized Normalize(IChart chart, IChartNormalized normalized);
        /// <summary>
        ///     ќпредел€ет признак того, что переданное представление нормализованного чарта поддерживаетс€ системой
        /// </summary>
        /// <param name="normalized">ѕредставление нормализованного чарта</param>
        /// <returns>ѕризнак того, что переданное представление нормализованного чарта поддерживаетс€ системой</returns>
        bool IsSupported(IChartNormalized normalized);
        /// <summary>
        ///     ќпредел€ет признак того, что переданное представление чарта поддерживаетс€ системой
        /// </summary>
        /// <param name="chart">ѕредставление чарта</param>
        /// <returns>ѕризнак того, что переданное представление чарта поддерживаетс€ системой</returns>
        bool IsSupported(IChart chart);
    }
}