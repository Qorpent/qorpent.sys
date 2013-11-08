using System;
using System.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsScaleNormalizer() {
            Code = FusionChartsNormalizerCodes.ScaleNormalizer;
            Area = ChartNormalizerArea.YScale;
            AddDependency(FusionChartsNormalizerCodes.FusionChartsValuesNormalizer);
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            normalized.AddScale(NormalizeYAxis(chart, normalized));
            return normalized;
        }

        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private ChartAbstractScale NormalizeYAxis(IChart chart, IChartNormalized normalized) {
            var normalizedScale = ScaleNormalizer.Normalize(
                new ScaleNormalizeClause(),
                normalized.GetFixedAttributes<decimal>(FusionChartApi.Set_Value).Select(Convert.ToDouble)
            );

            var scale = normalizedScale.RecommendedVariant;

            if (chart.Config.Height.ToInt() > 600) {
                var pixels = chart.Config.Height.ToDecimal()*0.8m;
                if (scale.Divline <= 3) {
                    scale.DoubleDivlines();
                }

                var shrinked = (scale.Maximal - scale.DivSize).ToDecimal();
                var pixnorm = ((scale.Maximal - scale.Minimal).ToDecimal()/pixels);

                if (
                    ((shrinked - scale.Minimal.ToDecimal())*pixnorm)  >= 
                    ((normalizedScale.Approximated.BaseMaximal - scale.Minimal).ToDecimal()*pixnorm + 40)
                ) {
                    scale.Maximal = Convert.ToDouble(shrinked);
                    scale.Divline--;
                }
            }


            /*
            var table = GetTable(chart, normalizedScale);
            if (table != null) {
                return new ChartAbstractScale {
                    ScaleType = ChartAbstractScaleType.Y,
                    NumDivLines = table.Divline,
                    MaxValue = table.Maximal,
                    MinValue = table.Minimal
                };
            }
            */
            return new ChartAbstractScale {
                ScaleType = ChartAbstractScaleType.Y,
                NumDivLines = normalizedScale.RecommendedVariant.Divline,
                MaxValue = normalizedScale.RecommendedVariant.Maximal,
                MinValue = normalizedScale.RecommendedVariant.Minimal
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private ScaleNormalizedVariant GetTable(IChart chart, ScaleNormalized normalized) {
            var tables = _approximated.Where(
                _ => _.Key.Similar(normalized.RecommendedVariant)
            ).SelectMany(
                _ => _.Value
            );

            if (!tables.Any()) {
                return null;
            }

            var el = tables.OrderByDescending(
                _ => _.Key
            ).FirstOrDefault(
                _ => chart.EnsureConfig().Height.ToInt() >= _.Key
            );

            return el.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        private readonly Table<ScaleNormalizedVariant, double, ScaleNormalizedVariant> _approximated = new Table<ScaleNormalizedVariant, double, ScaleNormalizedVariant> {
            {
                new ScaleNormalizedVariant { Maximal = 400, Minimal = 0, Divline = 2},
                new Table<double, ScaleNormalizedVariant> {{700, new ScaleNormalizedVariant { Maximal = 350, Minimal = 0, Divline = 7 }}}
            }, {
                new ScaleNormalizedVariant {Maximal = 1100, Minimal = 0, Divline = 5},
                new Table<double, ScaleNormalizedVariant> {
                    {800, new ScaleNormalizedVariant { Maximal = 1100, Minimal = 0, Divline = 10 }},
                    {200, new ScaleNormalizedVariant { Maximal = 1200, Minimal = 0, Divline = 3 }}
                }
            }, {
                new ScaleNormalizedVariant { Maximal = 27000, Minimal = 19000, Divline = 5 },
                new Table<double, ScaleNormalizedVariant> {
                    {600, new ScaleNormalizedVariant { Maximal = 27000, Minimal = 15000, Divline = 11 }},
                    {400, new ScaleNormalizedVariant { Maximal = 27000, Minimal = 15000, Divline = 3 }},
                    {200, new ScaleNormalizedVariant { Maximal = 30000, Minimal = 15000, Divline = 4 }}
                }
            }, {
                new ScaleNormalizedVariant {Maximal = 8100, Minimal = 7500, Divline = 3},
                new Table<double, ScaleNormalizedVariant> {
                    {200, new ScaleNormalizedVariant { Maximal = 8000, Minimal = 7500, Divline = 5 }}
                }
            }, {
                new ScaleNormalizedVariant { Maximal = 33000, Minimal = 30000, Divline = 3},
                new Table<double, ScaleNormalizedVariant> {
                    {200, new ScaleNormalizedVariant { Maximal = 32000, Minimal = 30000, Divline = 2}}
                }
            }, {
                new ScaleNormalizedVariant { Maximal = 33000, Minimal = 31000, Divline = 5},
                new Table<double, ScaleNormalizedVariant> {
                    {800, new ScaleNormalizedVariant { Maximal = 33000, Minimal = 30000, Divline = 5}},
                    {200, new ScaleNormalizedVariant { Maximal = 32000, Minimal = 30000, Divline = 2}}
                }
            }
        };
    }
    /// <summary>
    /// 
    /// </summary>
    internal class ChartExemplar {
        /// <summary>
        /// 
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double MaxValueTo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double HeightFrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double HeightTo { get; set; }
    }
}
