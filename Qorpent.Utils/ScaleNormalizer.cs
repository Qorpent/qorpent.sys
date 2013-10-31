using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = new ScaleNormalized(clause);
            var alimits = GetApproximatedScaleLimits(clause, values);
            ImproveApproximatedScaleLimits(clause, values);
            return result;
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
        /// <param name="clause"></param>
        /// <param name="scaleValues"></param>
        private void ImproveApproximatedScaleLimits(ScaleNormalizeClause clause, IEnumerable<double> scaleValues) {
            
        }
        /// <summary>
        ///     
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="scaleValues"></param>
        /// <returns></returns>
        private ApproximatedScaleLimits GetApproximatedScaleLimits(ScaleNormalizeClause clause, IEnumerable<double> scaleValues) {
            var limits = new ApproximatedScaleLimits(clause);
            var minimal = clause.UseMinimalValue ? clause.MinimalValue : scaleValues.Min();
            var maximal = clause.UseMaximalValue ? clause.MaximalValue : scaleValues.Max();



            return limits;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="withoutFraction"></param>
        /// <returns></returns>
        private IEnumerable<double> GetRandomDispersion(double from, double to, bool withoutFraction = true) {
            var single = Math.Round((to - from)/100);
            from = withoutFraction ? Math.Floor(from) : from;
            var last = from;
            
            while (last + single <= to) {
                last += single;
                if (single == 0.0) break;
                yield return withoutFraction ? last + single : Math.Floor(last + single);
            }
        }
    }
    /// <summary>
    ///     Контейнер для приблизительных значений шкалы сверху и снизу
    /// </summary>
    internal class ApproximatedScaleLimits {
        /// <summary>
        ///     Указатель на кляузу, к которой относятся лимиты
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     Набор минимальных значений шкалы
        /// </summary>
        public IEnumerable<double> Minimals { get; set; }
        /// <summary>
        ///     Набор максимальных значенй для шкалы
        /// </summary>
        public IEnumerable<double> Maximals { get; set; }
        /// <summary>
        ///     Контейнер для приблизительных значений шкалы сверху и снизу
        /// </summary>
        /// <param name="clause">Кляуза, к которой относятся значения</param>
        public ApproximatedScaleLimits(ScaleNormalizeClause clause) {
            Clause = clause;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SlickNumbers {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double GetFraction(double d) {
            var doubleString = d.ToString();
            var lastIndex = doubleString.LastIndexOf(".");
            double result = 0;
            if (lastIndex > 0) {
                var s = doubleString.Substring(lastIndex, doubleString.Length - lastIndex);
                result = Convert.ToDouble(s);
            }
            return result;
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
        ///     Внутренний список вариантов
        /// </summary>
        private readonly IList<ScaleNormalizedVariant> _variants = new List<ScaleNormalizedVariant>();
        /// <summary>
        ///     Исходная кляуза
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     Максимальное зачение шкалы
        /// </summary>
        public double Maximal {
            get { return Variants.FirstOrDefault().Maximal; }
        }
        /// <summary>
        ///     Минимальное значение шкалы
        /// </summary>
        public double Minimal {
            get { return Variants.FirstOrDefault().Minimal; }
        }
        /// <summary>
        ///     Количество дивлайнов
        /// </summary>
        public int Divline {
            get { return Variants.FirstOrDefault().Divline; }
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
    public class ScaleNormalizeClause {
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
        public double MinimalValue { get; private set; }
        /// <summary>
        ///     Максимальное значение
        /// </summary>
        public double MaximalValue { get; private set; }
        /// <summary>
        ///     Установка минимального значения
        /// </summary>
        /// <param name="minimal">Минимальное значение</param>
        public void SetMinimalValue(double minimal) {
            UseMinimalValue = true;
            MinimalValue = minimal;
        }
        /// <summary>
        ///     Установка максимального значения
        /// </summary>
        /// <param name="maximal">Максимальное значение</param>
        public void SetMaximalValue(double maximal) {
            UseMaximalValue = true;
            MaximalValue = maximal;
        }
    }
}
