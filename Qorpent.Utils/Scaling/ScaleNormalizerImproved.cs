﻿using System;
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
            var variants = normalized.Variants;

            // минималку в принципе не двигаем, так что её резольвим первой
            if (GetPixelApproximation(config, normalized, scale.Minimal, normalized.Approximated.BaseMinimal).Abs() < 5) {
                var sortedByMin = variants.Where(_ => _.Minimal < scale.Minimal).OrderByDescending(_ => _.Minimal);
                if (sortedByMin.Any()) {
                    scale.Minimal = sortedByMin.FirstOrDefault().Minimal;
                }
            }


            // разберёмся с максимальными значениями

                var sortedByMax = variants.Where(
                    _ => {
                        var delta = _.Maximal - _.Minimal;

                        if (delta.ToString().StartsWith(new[] { "11", "7", "17", "13" })) {
                            return false;
                        }

                        return true;
                    }
                ).OrderByDescending(_ => {
                    var n = _.DivSize.ToString();
                    if (n.StartsWith("5")) return 100;
                    if (n.StartsWith("2")) return 80;
                    if (n.StartsWith("1")) return 50;
                    return 0;
                }).OrderBy(_ => _.Maximal);
                if (sortedByMax.Any()) {
                    scale.Maximal = sortedByMax.FirstOrDefault().Maximal;
                }

            // считая, что максималка больше не изменится, разберёмся с минималками


            var preSortedDivlines = variants.Where(
                _ => _.Maximal.Equals(scale.Maximal) && _.Minimal.Equals(scale.Minimal)
            );

            // теперь изменим дивлайны
            if (config.Height.ToInt() >= 600) {
                var sortedByDivlines = preSortedDivlines.OrderByDescending(_ => {
                    var n = _.DivSize.ToString();
                    if (n.StartsWith("5")) {
                        return 100;
                    }

                    return 0;
                });
                if (sortedByDivlines.Any()) {
                    scale.Divline = sortedByDivlines.FirstOrDefault().Divline;
                }
            } else if (config.Height.ToInt() >= 400) {
                var sortedByDivlines = preSortedDivlines.Where(_ => _.Divline <= 6);
                if (sortedByDivlines.Any()) {
                    var better = sortedByDivlines.OrderBy(_ => {
                        var n = _.DivSize.ToString();
                        if (n.StartsWith("1")) {
                            return 100;
                        } else if (n.StartsWith("2")) {
                            return 50;
                        } else if (n.StartsWith("5")) {
                            return 20;
                        }

                        return 0;
                    });
                    if (better.Any()) {
                        scale.Divline = better.FirstOrDefault().Divline;
                    }
                }
            } else {
                var sortedByDivlines = preSortedDivlines.Where(_ => _.Divline <= 6);
                if (sortedByDivlines.Any()) {
                    var better = sortedByDivlines.OrderByDescending(_ => {
                        var n = _.Divline.ToString();
                        if (n.StartsWith("3")) {
                            return 100;
                        } else if (n.StartsWith("4")) {
                            return 50;
                        } else if (n.StartsWith("5")) {
                            return 20;
                        }

                        return 0;
                    });
                    if (better.Any()) {
                        scale.Divline = better.FirstOrDefault().Divline;
                    }
                }
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
    }
}
