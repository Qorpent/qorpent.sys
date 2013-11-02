using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Класс, обеспечивающий кэширование результатов работы нормализатора шкал
    /// </summary>
    public class ScaleNormalizeCache {
        private readonly IList<KeyValuePair<string, ScaleNormalizedVariant>> _cache;
        /// <summary>
        ///     Класс, обеспечивающий кэширование результатов работы нормализатора шкал
        /// </summary>
        public ScaleNormalizeCache() {
            _cache = new List<KeyValuePair<string, ScaleNormalizedVariant>>();
        }
        /// <summary>
        ///     Определяет признак наличия окргулённой шкалы в кэше в кэше
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        /// <returns>Признак того, что в кэше присутствует аппроксимированное значение для данного варианта</returns>
        public bool IsInCache(ScaleApproximated approximated) {
            var hash = GetHash(approximated);
            return _cache.Any(_ => _.Key == hash);
        }
        /// <summary>
        ///     Поднимает из кэша закэшированный вариант нормализации
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        /// <returns>Закэшированный вариант нормализации</returns>
        public ScaleNormalizedVariant Get(ScaleApproximated approximated) {
            var hash = GetHash(approximated);
            return _cache.FirstOrDefault(_ => _.Key == hash).Value;
        }
        /// <summary>
        ///     Опускает в кэш вариант нормализации графика
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        public void Put(ScaleApproximated approximated) {
            if (!approximated.IsDone) {
                throw new Exception("Can not drop to cache when approximation is not done");
            }
            
            Clean(approximated);
            _cache.Add(new KeyValuePair<string, ScaleNormalizedVariant>(approximated.Hash, approximated.Normalized.RecommendedVariant));
        }
        /// <summary>
        ///     Возвращает хэш аппроксимированного представления шкалы
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        /// <returns>Хэш аппроксимированного представления шкалы</returns>
        private string GetHash(ScaleApproximated approximated) {
            return approximated.Hash;
        }
        /// <summary>
        ///     Очистка вхождений данного представления в кэш
        /// </summary>
        /// <param name="approximated">Представление аппроксимированной и улучшенной шкалы</param>
        private void Clean(ScaleApproximated approximated) {
            var hash = GetHash(approximated);
            _cache.Where(_ => _.Key == hash).ToList().DoForEach(_ => _cache.Remove(_));
        }
    }
}