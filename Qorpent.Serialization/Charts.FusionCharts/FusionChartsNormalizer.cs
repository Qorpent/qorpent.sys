using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     
    /// </summary>
    public class FusionChartNormalizer : ConfigBase {
        private readonly IList<IChartNormalizer> _normalizers = new List<IChartNormalizer>();
        /// <summary>
        ///     Добавочная коллекция нормалайзеров
        /// </summary>
        public IList<IChartNormalizer> Normalizers {
            get { return _normalizers; }
        }
        /// <summary>
        ///     Произвести нормализацию чарта
        /// </summary>
        /// <param name="chart">Чарт</param>
        /// <returns>Нормализованный чарт</returns>
        public IChart Normalize(IChart chart) {
            var normalized = new ChartNormalized();
            
            GetWellKnownNormalizers().Union(Normalizers).DoForEach(
                _ => _.Normalize(chart, normalized)
            );

            normalized.Apply(chart);

            return chart;
        }
        /// <summary>
        ///     Создаёт перечисление хорошо известных нормалайзеров
        /// </summary>
        /// <returns>Перечисление хорошо известных нормалайзеров</returns>
        private IEnumerable<IChartNormalizer> GetWellKnownNormalizers() {
            yield return new FusionChartsScaleNormalizer();
            yield return new FusionChartsAnchorsNormalizer();
            yield return new FusionChartsColorNormalizer();
            yield return new FusionChartsNumberScalingNormalizer();
            yield return new FusionChartsPositionNormalizer();
        }
    }
}
