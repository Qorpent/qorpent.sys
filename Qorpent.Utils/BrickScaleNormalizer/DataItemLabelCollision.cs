using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Представление единичной коллизии (наезжания) лэйблов чарта
    /// </summary>
    internal class DataItemLabelCollision : IEnumerable<DataItem> {
        /// <summary>
        ///     Внутреннее перечисление конфликтов
        /// </summary>
        private readonly IEnumerable<DataItem> _conflicts;
        /// <summary>
        ///     Колонка, в которой обнаружена коллизия
        /// </summary>
        public DataItemColon Colon { get; private set; }
        /// <summary>
        ///     <see cref="DataItem"/>, у которой имеются конфликты
        /// </summary>
        public DataItem DataItem { get; private set; }
        /// <summary>
        ///     Перечисление <see cref="DataItem"/>, которые вступили в конфликт с <see cref="DataItem"/>
        /// </summary>
        public IEnumerable<DataItem> Conflicts {
            get { return _conflicts.Where(_ => DataItem.Temperature(_) > 0); }
        }
        /// <summary>
        ///     Указывает на то, что коллизий нет
        /// </summary>
        public bool IsEmpty {
            get { return !Conflicts.Any(); }
        }
        /// <summary>
        ///     Общая температура конфликта
        /// </summary>
        public decimal Temperature {
            get {
	            var conflicts = Conflicts.ToArray();
				if (conflicts.Length == 0) {
					return 0;
				}
	            return conflicts.Sum(_ => DataItem.Temperature(_))/conflicts.Length;
            }
        }
        /// <summary>
        ///     Представление единичной коллизии (наезжания) двух лэйблов чарта
        /// </summary>
        /// <param name="colon">Колонка, в которой обнаружена коллизия</param>
        /// <param name="dataItem"><see cref="DataItem"/>, у которой имеются конфликты</param>
        /// <param name="conflicts">Перечисление <see cref="DataItem"/>, которые вступили в конфликт с <see cref="DataItem"/></param>
        public DataItemLabelCollision(DataItemColon colon, DataItem dataItem, IEnumerable<DataItem> conflicts) {
            Colon = colon;
            DataItem = dataItem;
            _conflicts = conflicts;
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по всем участникам коллизии в виде <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по всем участникам коллизии в виде <see cref="DataItem"/></returns>
        public IEnumerator<DataItem> GetEnumerator() {
            return Conflicts.Union(new[] { DataItem }).GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по всем участникам коллизии в виде <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по всем участникам коллизии в виде <see cref="DataItem"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}