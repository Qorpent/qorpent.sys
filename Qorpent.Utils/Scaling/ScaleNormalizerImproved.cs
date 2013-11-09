using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Charts;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     пост нормалайзер для подгонки шкал для большей крисивости
    /// </summary>
    public static class ScaleNormalizerImproved {
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="config"></param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public static ScaleNormalized Normalize(ScaleNormalizeClause clause, IChartConfig config, IEnumerable<double> values) {
            var normalized = ScaleNormalizer.Normalize(clause, values);
            NormalizeYAxis(config, normalized);
            return normalized;
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="config"></param>
        /// <param name="values">Массив значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public static ScaleNormalized Normalize(ScaleNormalizeClause clause, IChartConfig config, params double[] values) {
            return Normalize(clause, config, values.AsEnumerable());
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="config"></param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public static ScaleNormalized Normalize(IChartConfig config, params double[] values) {
            var clause = new ScaleNormalizeClause {UseMaximalValue = false, UseMinimalValue = false};
            if (!string.IsNullOrWhiteSpace(config.MinValue)) {
                clause.UseMinimalValue = true;
                clause.MinimalValue = Convert.ToDouble(config.MinValue);
            }
            return Normalize(clause, config, values);
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="config"></param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public static ScaleNormalized Normalize(IChartConfig config, IEnumerable<double> values) {
            return Normalize(config, values.ToArray());
        }
        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private static void NormalizeYAxis(IChartConfig config, ScaleNormalized normalized) {
            var scale = normalized.RecommendedVariant;

            if (scale.Maximal <= 1000 && scale.Minimal >= 0) {
                scale.Minimal = 0;
            }

            if (scale.Minimal < 0 && scale.Maximal > 0) {
                if (scale.Maximal.Minimal(scale.Minimal.Abs()) / scale.Maximal.Maximal(scale.Minimal.Abs()).Abs() >= 0.8) {
                    var absMax = scale.Maximal.Maximal(scale.Minimal.Abs()).Abs();
                    scale.Minimal = absMax * (-1);
                    scale.Maximal = absMax;
                }
            }

            ResolveMinimals(config, normalized);
            ResolveMaximals(config, normalized);
            ResolveDivlines(config, normalized);
        }
        private static bool ShouldNormalizeMinimals(IChartConfig config, ScaleNormalized normalized) {
            if (normalized.RecommendedVariant.Minimal.Equals(0.0)) {
                return false;
            }

            return true;
        }
        /// <summary>
        ///     Увеличивает отступ от минимального значения вниз, если это требуется
        /// </summary>
        /// <param name="config">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        private static void ResolveMinimals(IChartConfig config, ScaleNormalized normalized) {
            if (!ShouldNormalizeMinimals(config, normalized)) {
                return;
            }

            while (true) {
                var currentPadding = GetPixelApproximation(config, normalized, normalized.RecommendedVariant.Minimal, normalized.Approximated.BaseMinimal).Abs();
                if (!(currentPadding < 20)) {
                    break;
                }

                normalized.RecommendedVariant.Minimal -= normalized.RecommendedVariant.DivSize;
                normalized.RecommendedVariant.Divline++;
            }
        }
        /// <summary>
        ///     Увеличивает отступ от максимального значения вверх, если это требуется
        /// </summary>
        /// <param name="config">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        private static void ResolveMaximals(IChartConfig config, ScaleNormalized normalized) {
            while (true) {
                var currentPadding = GetPixelApproximation(config, normalized, normalized.RecommendedVariant.Maximal, normalized.Approximated.BaseMaximal);
                if (!(currentPadding < 10)) {
                    break;
                }

                normalized.RecommendedVariant.Maximal += normalized.RecommendedVariant.DivSize;
                normalized.RecommendedVariant.Divline++;
            }
        }
        /// <summary>
        ///     Рассчитывает текущий отступ в пикселях реального максимального значения от граничного
        /// </summary>
        /// <param name="config">Представление исходного чарта</param>
        /// <param name="normalized">Представление нормализованного чарта</param>
        /// <param name="borderMax">Граничное максимальное значение</param>
        /// <param name="realMax">Реальное максимальное значение</param>
        /// <returns>Текущий отступ в пикселях реального максимального значения от граничного</returns>
        private static double GetPixelApproximation(IChartConfig config, ScaleNormalized normalized, double borderMax, double realMax) {
            return (borderMax - realMax) / GetPixnorm(config, normalized);
        }
        /// <summary>
        ///     Вычисляет приблизительную величину на пиксель
        /// </summary>
        /// <param name="config">Представление графика</param>
        /// <param name="normalized">Представление нормализованного графика</param>
        /// <returns>Приблизительная величина на пиксель</returns>
        private static double GetPixnorm(IChartConfig config, ScaleNormalized normalized) {
            return ((normalized.RecommendedVariant.Maximal - normalized.RecommendedVariant.Minimal) / config.Height.ToInt()).Abs();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="normalized"></param>
        private static void ResolveDivlines(IChartConfig config, ScaleNormalized normalized) {
            var scale = normalized.RecommendedVariant;

            if (scale.Minimal.Equals(0.0) && scale.Maximal <= 1000 && scale.Maximal >= 100) {
                scale.Divline = ((scale.Maximal/100) - 1).ToInt();
            }

            if (scale.Divline <= 3) {
                scale.DoubleDivlines();
            }

            if (config.Height.ToInt() >= 600) {
                while (true) {
                    var shrinked = (scale.Maximal - scale.DivSize);
                    var newMaximal = GetPixelApproximation(config, normalized, shrinked, normalized.Approximated.BaseMaximal);

                    if (newMaximal > 20) {
                        scale.Maximal = shrinked;
                        scale.Divline--;
                    } else {
                        break;
                    }
                }
            } else {
                if (normalized.RecommendedVariant.Minimal < 0) {
                    return;
                }
                var oldDivline = scale.Divline;

                if (scale.Divline > 6) {
                    if (TryDivlines(new[] {2,3,5}, normalized)) {
                        return;
                    }

                    scale.Divline = Math.Round(scale.Divline.ToDouble() / 2).ToInt();
                    while (true) {
                        if (scale.DivSize.IsRoundNumber(scale.DivSize.OrderEstimation())) {
                            break;
                        }
                        scale.Divline--;
                        if (scale.Divline == 0) {
                            break;
                        }
                    }
                }
                if (scale.Divline == 0) {
                    scale.Divline = oldDivline;
                }
            }
        }
        private static bool TryDivlines(IEnumerable<int> divlines, ScaleNormalized normalized) {
            var scale = normalized.RecommendedVariant;
            var oldDivline = scale.Divline;
            foreach (var divline in divlines) {
                scale.Divline = divline;
                if (scale.DivSize.IsRoundNumber(scale.DivSize.OrderEstimation())) {
                    return true;
                }
            }
            scale.Divline = oldDivline;
            return false;
        }
    }
}
