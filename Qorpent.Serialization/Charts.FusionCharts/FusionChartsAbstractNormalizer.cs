using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     Абстрактный класс нормализации чартов
    /// </summary>
    public abstract class FusionChartsAbstractNormalizer : ConfigBase, IChartNormalizer {
        /// <summary>
        ///     Область нормализации чарта
        /// </summary>
        public ChartNormalizerArea Area { get; protected set; }
        /// <summary>
        ///     Код нормалайзера
        /// </summary>
        public int Code { get; protected set; }
        /// <summary>
        ///     Зависимости нормалайзера от результата работы других нормалайзеров
        /// </summary>
        public IEnumerable<int> Dependencies { get; protected set; }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public abstract IChartNormalized Normalize(IChart chart, IChartNormalized normalized);
        /// <summary>
        ///     Определяет признак того, что переданное представление нормализованного чарта поддерживается системой
        /// </summary>
        /// <param name="normalized">Представление нормализованного чарта</param>
        /// <returns>Признак того, что переданное представление нормализованного чарта поддерживается системой</returns>
        public virtual bool IsSupported(IChartNormalized normalized) {
            return true;
        }
        /// <summary>
        ///     Определяет признак того, что переданное представление чарта поддерживается системой
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Признак того, что переданное представление чарта поддерживается системой</returns>
        public virtual bool IsSupported(IChart chart) {
            return true;
        }
    }
}