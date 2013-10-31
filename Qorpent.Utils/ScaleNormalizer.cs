using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils {
    /// <summary>
    ///     Утилита нормализации шкал для FusionCharts
    /// </summary>
    public class ScaleNormalizer {
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="values">Массив значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(ScaleNormalizeClause clause, params double[] values) {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Производит нормалзацию шкалы
        /// </summary>
        /// <param name="clause">Кляуза на нормализацию</param>
        /// <param name="values">Перечисление значений шкалы</param>
        /// <returns>Представление нормализованной шкалы</returns>
        public ScaleNormalized Normalize(ScaleNormalizeClause clause, IEnumerable<double> values) {
            return Normalize(clause, values.ToArray());
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
    }
    /// <summary>
    ///     Класс, представляющий нормализованную шкалу
    /// </summary>
    public class ScaleNormalized {
        /// <summary>
        ///     Внутренний список вариантов
        /// </summary>
        private readonly IList<ScaleNormalized> _variants = new List<ScaleNormalized>();
        /// <summary>
        ///     Исходная кляуза
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
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
        public double Divline { get; set; }
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
        public IEnumerable<ScaleNormalized> Variants {
            get { return _variants.AsEnumerable(); }
        }
        /// <summary>
        ///     Добавление варианта в нормализованное представление
        /// </summary>
        /// <param name="variant">Вариант</param>
        public void AddVariant(ScaleNormalized variant) {
            _variants.Add(variant);
        }
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
