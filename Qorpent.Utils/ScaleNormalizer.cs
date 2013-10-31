﻿using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    /// <summary>
    ///     Утилита нормализации шкал для FusionCharts
    /// </summary>
    public class ScaleNormalizer {
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(ScaleNormalizeClause clause, IEnumerable<double> values) {
            var approximated = new ApproximatedScaleLimits(clause, values, new ScaleNormalized(clause));

            BaseApproximation(approximated);
            GetApproximatedVariants(approximated);
            ImproveApproximatedVariants(approximated);
            BuildFinalVariants(approximated);
            SelectFinalVariant(approximated);
            
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
        ///     Производит улучшение апроксимированых значений 
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void ImproveApproximatedVariants(ApproximatedScaleLimits approximated) {
            if (!(approximated.BorderValue > 1 || approximated.BorderValue < -1)) {
                return;
            }

            var maximals = approximated.Maximals;
            var minimals = approximated.Minimals;

            //  немного улучшим значения округлениями и зачистим от повторов, использую Distinct()
            maximals = maximals.Select(_ => _.RoundUp(_.GetNumberOfDigits() - 1).ToDouble()).Distinct().ToList();
            minimals = minimals.Select(_ => _.RoundDown(_.GetNumberOfDigits() - 1).ToDouble()).Distinct().ToList();

            approximated.SetMaximals(maximals);
            approximated.SetMinimals(minimals);
        }
        /// <summary>
        ///     Возвращает разброс значений для дальнейшего запуска генетического алгоритма поиска лучшего решения
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        /// <returns></returns>
        private void GetApproximatedVariants(ApproximatedScaleLimits approximated) {
            var withFractions = approximated.BorderValue > 1 || approximated.BorderValue < -1;

            var step = approximated.BorderValue/20;
            step = step.RoundDown(step.GetNumberOfDigits() - 1);

            var minimals = SlickNumbers.GenerateLine(
                approximated.Minimal - step*20,
                (approximated.Minimal < 1000)
                    ?
                (approximated.Minimal.RoundDown(approximated.Minimal.GetNumberOfDigits()) - step)
                    :
                (approximated.Minimal + step * 20),
                step
            );

            var maximals = SlickNumbers.GenerateLine(
                approximated.Maximal,
                (approximated.Maximal < 1000)
                    ?
                (approximated.Maximal.RoundUp(approximated.Maximal.GetNumberOfDigits()) + step)
                    :
                (approximated.Maximal + step * 20),
                step
            );

            if (!withFractions) {
                minimals = minimals.Select(Math.Floor);
                maximals = maximals.Select(Math.Floor);
            }

            approximated.SetMinimals(minimals);
            approximated.SetMaximals(maximals);
        }
        /// <summary>
        ///     Подсчитывает приблизительные пределы, округляя нижнее и верхнее значение
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void BaseApproximation(ApproximatedScaleLimits approximated) {
            var maxDispersion = SlickNumbers.MaxDispersion(approximated.BaseValues);
            var minimal = approximated.BaseValues.Min();
            var maximal = approximated.BaseValues.Max();
            var mborder = (maximal - minimal)/5; // граничное значение — 20% от разрыва между минимальным и максимальным

            if (approximated.Clause.UseMinimalValue) {
                approximated.Minimal = approximated.Clause.MinimalValue;
            } else {
                if ((mborder >= maxDispersion) && (minimal >= 0)) {
                    approximated.Minimal = 0; // если разрыв слишком большой и значения больше нуля, то нижняя граница 0
                } else {
                    approximated.Minimal = minimal.RoundDown(minimal.GetNumberOfDigits() - 1);
                }
            }

            if (approximated.Clause.UseMaximalValue) {
                approximated.Maximal = approximated.Clause.MaximalValue;
            } else {
                approximated.Maximal = maximal.RoundUp(maximal.GetNumberOfDigits() - 1);
            }

            approximated.BorderValue = mborder;
        }
        /// <summary>
        ///     Собирает конечные варианты нормализации
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void BuildFinalVariants(ApproximatedScaleLimits approximated) {
            foreach (var maximal in approximated.Maximals) {
                foreach (var minimal in approximated.Minimals) {
                    var delta = maximal - minimal;
                    var divider = Math.Pow(10, delta.GetNumberOfDigits() - 2);
                    var rez = delta%divider;
                    if (rez == 0.0) {
                        var res = delta/divider;
                        if (res % 3 == 0.0) {
                            approximated.AddVariant(minimal, maximal, (res / 3).ToInt());
                        }

                        if (res % 6 == 0.0) {
                            approximated.AddVariant(minimal, maximal, (res / 6).ToInt());
                        }

                        if (res % 5 == 0.0) {
                            approximated.AddVariant(minimal, maximal, (res / 5).ToInt());
                        }
                    }
                }
            }
        }
        /// <summary>
        ///     Выбирает финальный вариант из всех представленных в качестве рекомендованного
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void SelectFinalVariant(ApproximatedScaleLimits approximated) {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    ///     Контейнер для приблизительных значений шкалы сверху и снизу
    /// </summary>
    internal class ApproximatedScaleLimits : ConfigBase {
        /// <summary>
        ///     Признак того, что миниальное значение установлено
        /// </summary>
        private bool _isMinimalSet;
        /// <summary>
        ///     Признак того, что максимальное значение было установлено
        /// </summary>
        private bool _isMaximalSet;
        /// <summary>
        ///     Признак того, что граничное значение установлено
        /// </summary>
        private bool _isBorderValueSet;
        /// <summary>
        ///     Указатель на кляузу, к которой относятся лимиты
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     Нормализованное представление шкалы
        /// </summary>
        public ScaleNormalized Normalized { get; private set; }
        /// <summary>
        ///     Минимальное значение — округлённое или выставленное пользователем
        /// </summary>
        public double Minimal {
            get {
                if (!_isMinimalSet) {
                    throw new Exception("The minimal value was not set");
                }

                return Get<double>("Minimal");
            }
            set {
                if (_isMinimalSet) {
                    throw new Exception("The minimal value was already set"); // логика проста: дальше — только варианты
                }

                _isMinimalSet = true;
                Set("Minimal", value);
            }
        }
        /// <summary>
        ///     Максимальное значение — округлённое или выставленное пользователем
        /// </summary>
        public double Maximal {
            get {
                if (!_isMaximalSet) {
                    throw new Exception("The maximal value was not set");
                }

                return Get<double>("Maximal");
            }
            set {
                if (_isMaximalSet) {
                    throw new Exception("The maximal values was already set"); // логика проста: дальше — только варианты
                }

                _isMaximalSet = true;
                Set("Maximal", value);
            }
        }
        /// <summary>
        ///     Граничное значение
        /// </summary>
        public double BorderValue {
            get {
                if (!_isBorderValueSet) {
                    throw new Exception("The border value value was not set"); // логика проста: дальше — только варианты
                }

                return Get<double>("BorderValue");
            }
            set {
                if (_isBorderValueSet) {
                    throw new Exception("The border value was already set");
                }

                _isBorderValueSet = true;
                Set("BorderValue", value);
            }
        }
        /// <summary>
        ///     Перечисление исходных значений шкалы
        /// </summary>
        public IEnumerable<double> BaseValues { get; private set; }
        /// <summary>
        ///     Набор минимальных значений шкалы
        /// </summary>
        public IEnumerable<double> Minimals { get; private set; }
        /// <summary>
        ///     Набор максимальных значенй для шкалы
        /// </summary>
        public IEnumerable<double> Maximals { get; private set; }

        /// <summary>
        ///     Контейнер для приблизительных значений шкалы сверху и снизу
        /// </summary>
        /// <param name="clause">Кляуза, к которой относятся значения</param>
        /// <param name="baseLimits">Базовые значения шкалы</param>
        /// <param name="normalized">Нормализованное представление шкалы</param>
        public ApproximatedScaleLimits(ScaleNormalizeClause clause, IEnumerable<double> baseLimits, ScaleNormalized normalized) {
            Clause = clause;
            BaseValues = baseLimits;
            Normalized = normalized;
        }
        /// <summary>
        ///     Установка перечисления максимальных значений
        /// </summary>
        /// <param name="maximals">Перечисление максимальных значений</param>
        public void SetMaximals(IEnumerable<double> maximals) {
            Maximals = maximals;
        }
        /// <summary>
        ///     Установка перечисления минимальных значений
        /// </summary>
        /// <param name="minimals">Перечисление минимальных значений</param>
        public void SetMinimals(IEnumerable<double> minimals) {
            Minimals = minimals;
        }
        /// <summary>
        ///     Добавление нормализованного варианта
        /// </summary>
        /// <param name="variant">Представление варианта</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            Normalized.AddVariant(variant);
        }
        /// <summary>
        ///     Добавление нормализованного варианта
        /// </summary>
        /// <param name="minimal">Минимальное значение</param>
        /// <param name="maximal">Максимальное значение</param>
        /// <param name="divlines">Количество дивлайнов</param>
        public void AddVariant(double minimal, double maximal, int divlines) {
            Normalized.AddVariant(new ScaleNormalizedVariant { Divline = divlines, Minimal = minimal, Maximal = maximal });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SlickNumbers {
        /// <summary>
        ///     Генерирует ряд из чисел с шагом
        /// </summary>
        /// <param name="from">Начало ряда (включительно)</param>
        /// <param name="to">Предел ряда ряда</param>
        /// <param name="step">Шаг</param>
        /// <returns></returns>
        public static IEnumerable<double> GenerateLine(double from, double to, double step) {
            if (Math.Abs(step - (double.MinValue - double.MaxValue)) <= 0) {
                throw new Exception("Infinity loop detected");
            }

            if (to < from) {
                throw new Exception("Infinity loop detected");
            }

            var current = from;
            while (current <= to) {
                yield return current;
                current += step;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static IEnumerable<double> Dispersion(IEnumerable<double> numbers) {
            var array = numbers.ToArray();
            for (var i = 1; i < array.Length; i++) {
                yield return array[i] - array[i - 1];
            }
        }
        /// <summary>
        ///     Поиск максимального разброса между значениями
        /// </summary>
        /// <param name="numbers">Пречисление чисел</param>
        /// <returns>Максимальный разброс между двумя соседними числами</returns>
        public static double MaxDispersion(IEnumerable<double> numbers) {
            return Dispersion(numbers.OrderBy(_ => _)).Max();
        }
        /// <summary>
        ///     Поиск минимального разброса между значениями
        /// </summary>
        /// <param name="numbers">Пречисление чисел</param>
        /// <returns>Минимальный разброс между двумя соседними числами</returns>
        public static double MinDispersion(IEnumerable<double> numbers) {
            return Dispersion(numbers.OrderBy(_ => _)).Min();
        }
        /// <summary>
        ///     Сделать охуенно
        /// </summary>
        /// <param name="notSlick"></param>
        /// <returns></returns>
        public static IEnumerable<double> SlickSort(IEnumerable<double> notSlick) {
            notSlick = notSlick.OrderByDescending(_ => _);
            var t = new List<double>();
            for (var i = notSlick.Max().GetNumberOfDigits() - 1; i > 0; i--) {
                foreach (var d in notSlick.Where(_ => _%Math.Pow(10, i) == 0 && !t.Contains(_))) {
                     t.Add(d);
                }
            }

            var sublist = notSlick.Where(_ => !t.Contains(_));

            if (!sublist.Any()) return t;

            for (var i = sublist.Max().GetNumberOfDigits() - 1; i >= 0; i--) {
                foreach (var d in sublist.Where(_ => {
                    if (t.Contains(_)) {
                        return false;
                    }
                    var divider = Math.Pow(10, i)*2;
                    var result = _%divider;
                    var res = result == 0.0;
                    return res;
                })) {
                    t.Add(d);
                }
            }

            sublist = notSlick.Where(_ => !t.Contains(_));

            return t.Union(sublist);
        }
    }
    /// <summary>
    ///     Класс, представляющий нормализованную шкалу
    /// </summary>
    public class ScaleNormalized {
        /// <summary>
        ///     Рекомендованный вариант
        /// </summary>
        private ScaleNormalizedVariant _recommendedVariant;
        /// <summary>
        ///     Внутренний список вариантов
        /// </summary>
        private readonly IList<ScaleNormalizedVariant> _variants = new List<ScaleNormalizedVariant>();
        /// <summary>
        ///     Исходная кляуза
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     Рекомендованный вариант
        /// </summary>
        public ScaleNormalizedVariant RecommendedVariant {
            get { return _recommendedVariant; }
        }
        /// <summary>
        ///     Максимальное зачение шкалы
        /// </summary>
        public double Maximal {
            get { return _recommendedVariant.Maximal; }
        }
        /// <summary>
        ///     Минимальное значение шкалы
        /// </summary>
        public double Minimal {
            get { return _recommendedVariant.Minimal; }
        }
        /// <summary>
        ///     Количество дивлайнов
        /// </summary>
        public int Divline {
            get { return _recommendedVariant.Divline; }
        }
        /// <summary>
        ///     Класс, представляющий нормализованную шкалу
        /// </summary>
        /// <param name="clause">Исходная кляуза</param>
        public ScaleNormalized(ScaleNormalizeClause clause) {
            Clause = clause;
        }
        /// <summary>
        ///     Перечисление вариантов нормализации, сочтённых нормализатором менее подходящими
        /// </summary>
        public IEnumerable<ScaleNormalizedVariant> Variants {
            get { return _variants.AsEnumerable(); }
        }
        /// <summary>
        ///     Добавление варианта в нормализованное представление
        /// </summary>
        /// <param name="variant">Вариант</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            _variants.Add(variant);
        }
        /// <summary>
        ///     Установка рекомендованного варианта
        /// </summary>
        /// <param name="recommendedVariant">Рекомендованный вариант</param>
        public void SetRecommendedVariant(ScaleNormalizedVariant recommendedVariant) {
            _recommendedVariant = recommendedVariant;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScaleNormalizedVariant {
        /// <summary>
        ///     Максимальное зачение шкалы
        /// </summary>
        public double Maximal { get; set; }
        /// <summary>
        ///     Минимальное значение шкалы
        /// </summary>
        public double Minimal { get; set; }
        /// <summary>
        ///     Количество дивлайнов
        /// </summary>
        public int Divline { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScaleNormalizeClause : ConfigBase {
        /// <summary>
        ///     Признак того, что минимальное значение было утсановлено
        /// </summary>
        private bool _isMinimalValueSet;
        /// <summary>
        ///     Признак того, что максимальное значение было установлено
        /// </summary>
        private bool _isMaximalValueSet;
        /// <summary>
        ///     Признак того, что нужно использовать установленное минимальное значение
        /// </summary>
        public bool UseMinimalValue { get; set; }
        /// <summary>
        ///     Признак того, что нужно использовать установленное максимальное значение
        /// </summary>
        public bool UseMaximalValue { get; set; }

        /// <summary>
        ///     Минимальное значение
        /// </summary>
        public double MinimalValue {
            get {
                if (!_isMinimalValueSet) {
                    throw new Exception("The minimal value was not set");
                }

                return Get<double>("MinimalValue");
            }
            set {
                _isMinimalValueSet = true;
                Set("MinimalValue", value);
            }
        }
        /// <summary>
        ///     Максимальное значение
        /// </summary>
        public double MaximalValue {
            get {
                if (!_isMaximalValueSet) {
                    throw new Exception("The maximal value was not set");
                }

                return Get<double>("MaximalValue");
            }
            set {
                _isMaximalValueSet = true;
                Set("MaximalValue", value);
            }
        }
    }
}
