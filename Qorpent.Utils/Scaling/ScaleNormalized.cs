using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Класс, представляющий нормализованную шкалу
    /// </summary>
    public class ScaleNormalized {
        /// <summary>
        ///     Признак того, что нормализация завершена
        /// </summary>
        private bool _isDone;
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
        ///     Возвращает признак завершённости обработки шкалы
        /// </summary>
        public bool IsDone {
            get { return _isDone; }
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
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _variants.Add(variant);
        }
        /// <summary>
        ///     Установка рекомендованного варианта
        /// </summary>
        /// <param name="recommendedVariant">Рекомендованный вариант</param>
        public void SetRecommendedVariant(ScaleNormalizedVariant recommendedVariant) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _recommendedVariant = recommendedVariant;
        }
        /// <summary>
        ///     Указывает признак того, что обработка завершена
        /// </summary>
        public void Done() {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _isDone = true;
        }
    }
}