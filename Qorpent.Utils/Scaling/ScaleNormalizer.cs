using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Утилита нормализации шкал для FusionCharts
    /// </summary>
    public class ScaleNormalizer {
        /// <summary>
        ///     Код шага базовой аппроксимации
        /// </summary>
        public const int BaseApproximationCode = 0;
        /// <summary>
        ///     Код шага получения вариантов нормализации
        /// </summary>
        public const int GetApproximatedVariantsCode = 1;
        /// <summary>
        ///     Код шага улучшения полученных вариантов нормализации
        /// </summary>
        public const int ImproveApproximatedVariantsCode = 2;
        /// <summary>
        ///     Код шага сборки полученных вариантов нормализации
        /// </summary>
        public const int BuildFinalVariantsCode = 3;
        /// <summary>
        ///     Код шага выборки рекомендованного варианта нормализации
        /// </summary>
        public const int SelectFinalVariantCode = 4;
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(ScaleNormalizeClause clause, IEnumerable<double> values) {
            var approximated = GetApproximatedBase(clause, values);
            approximated.ErrorBahavior = _ => {
                if (_.Normalized == null) {
                    return;
                }

                if (!_.BaseValues.Any()) {
                    _.Normalized.SetRecommendedVariant(new ScaleNormalizedVariant {
                        Divline = 0,
                        Minimal = double.MinValue,
                        Maximal = double.MaxValue
                    });

                    return;
                }

                _.Normalized.SetRecommendedVariant(new ScaleNormalizedVariant {
                    Divline = 3,
                    Maximal = _.BaseValues.Max(),
                    Minimal = _.BaseValues.Min()
                });
            };

            var steps = GetApproximationSteps();

            try {
                foreach (var step in steps) {
                    step.Value(approximated);
                    var appendixes = clause.Appendixes.Where(_ => _.Key == step.Key).ToList();
                    foreach (var appendix in appendixes) {
                        appendix.Value(approximated);
                    }
                }
            } catch (Exception e) {
                approximated.Error(e, true);
            }

            return approximated.Normalized;
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="values">Массив значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(ScaleNormalizeClause clause, params double[] values) {
            return Normalize(clause, values.AsEnumerable());
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(params double[] values) {
            return Normalize(new ScaleNormalizeClause { UseMaximalValue = false, UseMinimalValue = false }, values);
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(IEnumerable<double> values) {
            return Normalize(values.ToArray());
        }
        /// <summary>
        ///     Производит добивку коллекции модулей нормализаци системными вариантами для более тонкой нормализации
        ///     на разных шагах
        /// </summary>
        /// <param name="clause">Исходный запрос на нормализацию</param>
        private void InsertBaseAppendixes(ScaleNormalizeClause clause) {
            if (!clause.RunSlickNormalization) {
                return;
            }

            clause.AddAppendix(ImproveApproximatedVariantsCode, _ => {
                if (_.Maximal < 1000) {
                    _.SetMinimals(new ArraySegment<double>(new[] { 0.0 }));
                    _.SetMaximals(SlickNumbers.GenerateLine(_.Maximal, 1000, 90).Select(
                        __ => __.RoundUp(__.GetNumberOfDigits() - 1).ToDouble())
                    );
                }
            }).AddAppendix(SelectFinalVariantCode, _ => {
                if (_.Maximal < 1000 && _.Minimal >= 0) {
                    if (_.Maximal + 20 <= _.Maximal.RoundUp(_.Maximal.GetNumberOfDigits())) {
                        _.Normalized.SetRecommendedVariant(new ScaleNormalizedVariant {
                            Minimal = _.Normalized.RecommendedVariant.Minimal,
                            Maximal = _.Maximal.RoundUp(_.Maximal.GetNumberOfDigits()),
                            Divline = _.Maximal.RoundUp(_.Maximal.GetNumberOfDigits()) / 100 - 1
                        });
                    }
                }
            });
        }
        /// <summary>
        ///     Возвращает перечисление шагов нормализации шкалы
        /// </summary>
        /// <returns>Перечисление шагов нормализации шкалы</returns>
        private IEnumerable<KeyValuePair<int, Action<ScaleApproximated>>> GetApproximationSteps() {
            yield return new KeyValuePair<int, Action<ScaleApproximated>>(BaseApproximationCode, BaseApproximation);
            yield return new KeyValuePair<int, Action<ScaleApproximated>>(GetApproximatedVariantsCode, GetApproximatedVariants);
            yield return new KeyValuePair<int, Action<ScaleApproximated>>(ImproveApproximatedVariantsCode, ImproveApproximatedVariants);
            yield return new KeyValuePair<int, Action<ScaleApproximated>>(BuildFinalVariantsCode, BuildFinalVariants);
            yield return new KeyValuePair<int, Action<ScaleApproximated>>(SelectFinalVariantCode, SelectFinalVariant);
        }
        /// <summary>
        ///     Подготавливает базу для нормализации
        /// </summary>
        /// <param name="clause">Исходный запрос на нормализацию</param>
        /// <param name="baseValues">Перечисление базовых значений</param>
        /// <returns>Представление аппроксимированной и улучшенной шкалы</returns>
        private ScaleApproximated GetApproximatedBase(ScaleNormalizeClause clause, IEnumerable<double> baseValues) {
            InsertBaseAppendixes(clause);
            return new ScaleApproximated(clause, baseValues, new ScaleNormalized(clause));
        }
        /// <summary>
        ///     Производит улучшение апроксимированых значений 
        /// </summary>
        /// <param name="scaleApproximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void ImproveApproximatedVariants(ScaleApproximated scaleApproximated) {
            if (!(scaleApproximated.BorderValue > 1 || scaleApproximated.BorderValue < -1)) {
                return;
            }

            var maximals = scaleApproximated.Maximals;
            var minimals = scaleApproximated.Minimals;

            //  немного улучшим значения округлениями и зачистим от повторов, использую Distinct()
            maximals = maximals.Select(_ => _.RoundUp(_.GetNumberOfDigits() - 1).ToDouble()).Distinct().ToList();
            minimals = minimals.Select(_ => _.RoundDown(_.GetNumberOfDigits() - 1).ToDouble()).Distinct().ToList();

            scaleApproximated.SetMaximals(maximals);
            scaleApproximated.SetMinimals(minimals);
        }
        /// <summary>
        ///     Возвращает разброс значений для дальнейшего запуска генетического алгоритма поиска лучшего решения
        /// </summary>
        /// <param name="scaleApproximated">Представление аппроксимированной и улучшенной шкалы</param>
        /// <returns></returns>
        private void GetApproximatedVariants(ScaleApproximated scaleApproximated) {
            var withFractions = scaleApproximated.BorderValue > 1 || scaleApproximated.BorderValue < -1;

            var step = scaleApproximated.BorderValue/20;
            step = step.RoundDown(step.GetNumberOfDigits() - 1);

            var minimals = SlickNumbers.GenerateLine(
                scaleApproximated.Minimal - step*20,
                (scaleApproximated.Minimal < 1000)
                    ?
                (scaleApproximated.Minimal.RoundDown(scaleApproximated.Minimal.GetNumberOfDigits()) - step)
                    :
                (scaleApproximated.Minimal + step * 20),
                step
            );

            var maximals = SlickNumbers.GenerateLine(
                scaleApproximated.Maximal,
                (scaleApproximated.Maximal < 1000)
                    ?
                (scaleApproximated.Maximal.RoundUp(scaleApproximated.Maximal.GetNumberOfDigits()) + step)
                    :
                (scaleApproximated.Maximal + step * 20),
                step
            );

            if (!withFractions) {
                minimals = minimals.Select(Math.Floor);
                maximals = maximals.Select(Math.Floor);
            }

            scaleApproximated.SetMinimals(minimals);
            scaleApproximated.SetMaximals(maximals);
        }
        /// <summary>
        ///     Подсчитывает приблизительные пределы, округляя нижнее и верхнее значение
        /// </summary>
        /// <param name="scaleApproximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void BaseApproximation(ScaleApproximated scaleApproximated) {
            var maxDispersion = SlickNumbers.MaxDispersion(scaleApproximated.BaseValues);
            var minimal = scaleApproximated.BaseValues.Min();
            var maximal = scaleApproximated.BaseValues.Max();
            var mborder = (maximal - minimal)/5; // граничное значение — 20% от разрыва между минимальным и максимальным

            if (scaleApproximated.Clause.UseMinimalValue) {
                scaleApproximated.Minimal = scaleApproximated.Clause.MinimalValue;
            } else {
                if ((mborder >= maxDispersion) && (minimal >= 0)) {
                    scaleApproximated.Minimal = 0; // если разрыв слишком большой и значения больше нуля, то нижняя граница 0
                } else {
                    scaleApproximated.Minimal = minimal.RoundDown(minimal.GetNumberOfDigits() - 1);
                }
            }

            if (scaleApproximated.Clause.UseMaximalValue) {
                scaleApproximated.Maximal = scaleApproximated.Clause.MaximalValue;
            } else {
                scaleApproximated.Maximal = maximal.RoundUp(maximal.GetNumberOfDigits() - 1);
            }

            scaleApproximated.BorderValue = mborder;
        }
        /// <summary>
        ///     Собирает конечные варианты нормализации
        /// </summary>
        /// <param name="scaleApproximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void BuildFinalVariants(ScaleApproximated scaleApproximated) {
            foreach (var maximal in scaleApproximated.Maximals) {
                foreach (var minimal in scaleApproximated.Minimals) {
                    var delta = maximal - minimal;
                    var divider = Math.Pow(10, delta.GetNumberOfDigits() - 2);
                    var rez = delta%divider;
                    if (rez == 0.0) {
                        var res = delta/divider;
                        if (res % 3 == 0.0) {
                            scaleApproximated.AddVariant(minimal, maximal, 3.ToInt());
                        }

                        if (res % 6 == 0.0) {
                            scaleApproximated.AddVariant(minimal, maximal, 6);
                        }

                        if (res % 5 == 0.0) {
                            scaleApproximated.AddVariant(minimal, maximal, 5);
                        }
                    }
                }
            }
        }
        /// <summary>
        ///     Выбирает финальный вариант из всех представленных в качестве рекомендованного
        /// </summary>
        /// <param name="scaleApproximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void SelectFinalVariant(ScaleApproximated scaleApproximated) {
            scaleApproximated.Normalized.SetRecommendedVariant(scaleApproximated.Normalized.Variants.FirstOrDefault());
        }
    }
}
