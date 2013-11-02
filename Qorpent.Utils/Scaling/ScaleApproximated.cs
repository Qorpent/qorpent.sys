using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Контейнер для приблизительных значений шкалы сверху и снизу
    /// </summary>
    public class ScaleApproximated : ConfigBase {
        /// <summary>
        ///     Закэшированное максимальное значение базовой коллекции
        /// </summary>
        private readonly double _baseMaximal;
        /// <summary>
        ///     Закэшированное минимальное значение базовой коллекции
        /// </summary>
        private readonly double _baseMinimal;
        /// <summary>
        ///     Признак того, что обработка шкалы завершена
        /// </summary>
        private bool _isDone;
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
        ///     Исключение, пойманное в момент возникновения ошибки, на наличие которой указывает
        ///     <see cref="IsError"/>
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        ///     Признак того, что при нормализации была выявлена фатальная ошибка
        ///     шаттная процедура нормализации не была проведена
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        ///     Признак того, что при попытке среагировать на фатальную ошибку, произошёл сбой
        /// </summary>
        public bool IsErrorBahaviorError { get; private set; }
        /// <summary>
        ///     Указатель на кляузу, к которой относятся лимиты
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     Нормализованное представление шкалы
        /// </summary>
        public ScaleNormalized Normalized { get; private set; }
        /// <summary>
        ///     Действие, запускаемое при возникновении фатальной ошибки
        /// </summary>
        public Action<ScaleApproximated> ErrorBahavior { get; set; }
        /// <summary>
        ///     Возвращает признак завершённости 
        /// </summary>
        public bool IsDone {
            get { return _isDone; }
        }
        /// <summary>
        ///     Минимальное значение из базовых значений
        /// </summary>
        public double BaseMinimal {
            get { return _baseMinimal; }
        }
        /// <summary>
        ///     Максимальное значение из базовых значений
        /// </summary>
        public double BaseMaximal {
            get { return _baseMaximal; }
        }
        /// <summary>
        ///     Хэш текущей аппроксимации
        /// </summary>
        public string Hash {
            get { return MatchHash(); }
        }
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
        /// <param name="baseValues">Базовые значения шкалы</param>
        /// <param name="normalized">Нормализованное представление шкалы</param>
        public ScaleApproximated(ScaleNormalizeClause clause, IEnumerable<double> baseValues, ScaleNormalized normalized) {
            Clause = clause;
            BaseValues = baseValues;
            Normalized = normalized;

            _baseMaximal = BaseValues.Max();
            _baseMinimal = BaseValues.Min();
        }
        /// <summary>
        ///     Установка перечисления максимальных значений
        /// </summary>
        /// <param name="maximals">Перечисление максимальных значений</param>
        public void SetMaximals(IEnumerable<double> maximals) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Maximals = maximals;
        }
        /// <summary>
        ///     Установка перечисления минимальных значений
        /// </summary>
        /// <param name="minimals">Перечисление минимальных значений</param>
        public void SetMinimals(IEnumerable<double> minimals) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Minimals = minimals;
        }
        /// <summary>
        ///     Добавление нормализованного варианта
        /// </summary>
        /// <param name="variant">Представление варианта</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Normalized.AddVariant(variant);
        }
        /// <summary>
        ///     Добавление нормализованного варианта
        /// </summary>
        /// <param name="minimal">Минимальное значение</param>
        /// <param name="maximal">Максимальное значение</param>
        /// <param name="divlines">Количество дивлайнов</param>
        public void AddVariant(double minimal, double maximal, int divlines) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            AddVariant(new ScaleNormalizedVariant { Divline = divlines, Minimal = minimal, Maximal = maximal });
        }
        /// <summary>
        ///     Установка состояния фатальной ошибки
        /// </summary>
        /// <param name="exception">Исключение, представляющее ошибку</param>
        /// <param name="runErrorBehavior">Признак того, что нужно запустить алгоритм реакции на сбой</param>
        public void Error(Exception exception, bool runErrorBehavior) {
            IsError = true;
            _isDone = false;
            Exception = exception;

            if (runErrorBehavior) {
                if (ErrorBahavior == null) {
                    return;
                }

                try {
                    ErrorBahavior(this);
                } catch {
                    IsErrorBahaviorError = true;
                }
            }
        }
        /// <summary>
        ///     Устанавливает признак того, что обработка завершена
        /// </summary>
        public void Done() {
            Normalized.Done();
            _isDone = true;
        }
        /// <summary>
        ///     Подсчитывает хэш аппроксимированного представления
        /// </summary>
        /// <returns>Хэш аппроксимированного представления</returns>
        private string MatchHash() {
            return GetHashCode().ToString();
        }
    }
}