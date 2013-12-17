using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Представление графика на кирпичах (привет, Бендер!)
    /// </summary>
    public class BrickChart : BrickDataSet {
        /// <summary>
        ///     Внутренее хранилище мета-информации
        /// </summary>
        private readonly Dictionary<string, object> _meta = new Dictionary<string, object>();
        /// <summary>
        ///     Список стилей графика
        /// </summary>
        private readonly List<BrickChartStyle> _styles = new List<BrickChartStyle>();
        /// <summary>
        ///     Список эпликейшэнов графика
        /// </summary>
        private readonly List<BrickChartApplication> _applications = new List<BrickChartApplication>();
        /// <summary>
        ///     Мета-информация графика в виде пар типа ключ:значение
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Meta {
            get { return _meta.AsEnumerable(); }
        }
        /// <summary>
        ///     Стили графика
        /// </summary>
        public IEnumerable<BrickChartStyle> Styles {
            get { return _styles.AsEnumerable(); }
        }
        /// <summary>
        ///     Установка мета-информации графика
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="value">Значение</param>
        public void SetMeta(string key, object value) {
            if (_meta.ContainsKey(key)) {
                _meta[key] = value;
            } else {
                _meta.Add(key, value);
            }
        }
        /// <summary>
        ///     Уничтожение значения мета-информации по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        public void UnsetMeta(string key) {
            _meta.Remove(key);
        }
        /// <summary>
        ///     Получение значения мета-информации по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Значение</returns>
        public object GetMeta(string key) {
            if (_meta.ContainsKey(key)) {
                return _meta[key];
            }

            return string.Empty;
        }
        /// <summary>
        ///     Добавление стиля в граяик
        /// </summary>
        /// <param name="style">Стиль графика</param>
        public void AddStyle(BrickChartStyle style) {
            _styles.Add(style);
        }
        /// <summary>
        ///     Удаление стилей из графика по имени стиля
        /// </summary>
        /// <param name="name">Имя стиля</param>
        public void RemoveStyle(string name) {
            _styles.RemoveAll(_ => _.Name.Equals(name));
        }
        /// <summary>
        ///     Добавление эпликейшена в график
        /// </summary>
        /// <param name="application">Эпликейшен</param>
        public void AddApplication(BrickChartApplication application) {
            _applications.Add(application);
        }
        /// <summary>
        ///     Сборка графика в JSON
        /// </summary>
        /// <returns>JSON-представление графика</returns>
        public string Json() {
            throw new NotImplementedException();
        }
    }
}
