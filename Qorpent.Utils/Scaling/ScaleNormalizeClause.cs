using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     ѕредставление запроса на нормалзацию
    /// </summary>
    public class ScaleNormalizeClause : ConfigBase {
        /// <summary>
        ///     ƒополнительные действи€, запускаемые каждый раз после выполнени€ шага с указанным кодом
        /// </summary>
        private IList<KeyValuePair<int, Action<ScaleApproximated>>> _appendixes = new List<KeyValuePair<int, Action<ScaleApproximated>>>();
        /// <summary>
        ///     ѕризнак того, что минимальное значение было утсановлено
        /// </summary>
        private bool _isMinimalValueSet;
        /// <summary>
        ///     ѕризнак того, что максимальное значение было установлено
        /// </summary>
        private bool _isMaximalValueSet;
        /// <summary>
        ///     ѕризнак того, что нужно использовать установленное минимальное значение
        /// </summary>
        public bool UseMinimalValue { get; set; }
        /// <summary>
        ///     ѕризнак того, что нужно использовать установленное максимальное значение
        /// </summary>
        public bool UseMaximalValue { get; set; }
        /// <summary>
        ///     ѕризнак того, что нужно проводить тонкую нормализацию при помощи дополнительных <see cref="Appendixes"/>,
        ///     вставл€емых компил€тором плана нормализации
        /// </summary>
        public bool RunSlickNormalization { get; set; }
        /// <summary>
        ///     ƒополнительные действи€, запускаемые каждый раз после выполнени€ шага с указанным кодом
        /// </summary>
        public IEnumerable<KeyValuePair<int, Action<ScaleApproximated>>> Appendixes {
            get { return _appendixes.AsEnumerable(); }
        }
        /// <summary>
        ///     ћинимальное значение
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
        ///     ћаксимальное значение
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
        ///     ѕредставление запроса на нормалзацию
        /// </summary>
        public ScaleNormalizeClause() {
            RunSlickNormalization = true;
        }
        /// <summary>
        ///     ƒобавление дополнительного действи€ в коллекцию
        /// </summary>
        /// <param name="appendix">ѕара, представл€юща€ код шага и действие</param>
        public ScaleNormalizeClause AddAppendix(KeyValuePair<int, Action<ScaleApproximated>> appendix) {
            _appendixes.Add(appendix);
            return this;
        }
        /// <summary>
        ///     ƒобавление дополнительного действи€ в коллекцию
        /// </summary>
        /// <param name="stepCode"> од шага, с которым ассоциированно действие</param>
        /// <param name="appendix">ƒействие</param>
        public ScaleNormalizeClause AddAppendix(int stepCode, Action<ScaleApproximated> appendix) {
            return AddAppendix(new KeyValuePair<int, Action<ScaleApproximated>>(stepCode, appendix));
        }
    }
}