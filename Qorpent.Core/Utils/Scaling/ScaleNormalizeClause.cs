using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Представление запроса на нормалзацию
    /// </summary>
    public class ScaleNormalizeClause : ConfigBase {
        /// <summary>
        ///     Дополнительные действия, запускаемые каждый раз после выполнения шага с указанным кодом
        /// </summary>
        private readonly IList<KeyValuePair<int, Action<ScaleApproximated>>> _appendixes;
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
        ///     Признак того, что нужно проводить тонкую нормализацию при помощи дополнительных <see cref="Appendixes"/>,
        ///     вставляемых компилятором плана нормализации
        /// </summary>
        public bool RunSlickNormalization { get; set; }
        /// <summary>
        ///     Дополнительные действия, запускаемые каждый раз после выполнения шага с указанным кодом
        /// </summary>
        public IEnumerable<KeyValuePair<int, Action<ScaleApproximated>>> Appendixes {
            get { return _appendixes.AsEnumerable(); }
        }
        /// <summary>
        ///     Признак того, что нужно использовать кэш
        /// </summary>
        public bool UseCache { get; set; }
        /// <summary>
        ///     Хэш
        /// </summary>
        public string Hash {
            get { return MatchHash(); }
        }
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
        /// <summary>
        ///     Представление запроса на нормалзацию
        /// </summary>
        public ScaleNormalizeClause() {
            _appendixes = new List<KeyValuePair<int, Action<ScaleApproximated>>>();

            RunSlickNormalization = true;
            UseCache = false;
        }
        /// <summary>
        ///     Добавление дополнительного действия в коллекцию
        /// </summary>
        /// <param name="appendix">Пара, представляющая код шага и действие</param>
        public ScaleNormalizeClause AddAppendix(KeyValuePair<int, Action<ScaleApproximated>> appendix) {
            _appendixes.Add(appendix);
            return this;
        }
        /// <summary>
        ///     Добавление дополнительного действия в коллекцию
        /// </summary>
        /// <param name="stepCode">Код шага, с которым ассоциированно действие</param>
        /// <param name="appendix">Действие</param>
        public ScaleNormalizeClause AddAppendix(int stepCode, Action<ScaleApproximated> appendix) {
            return AddAppendix(new KeyValuePair<int, Action<ScaleApproximated>>(stepCode, appendix));
        }
        /// <summary>
        ///     Подсчитывает хэш запроса на нормализацию
        /// </summary>
        /// <returns>Хэш запроса на нормализацию</returns>
        private string MatchHash() {
            return GetHashCode().ToString();
        }
    }
}