using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Колонка значений графика для расчета их позиций
    /// </summary>
    public class DataItemColon : IEnumerable<DataItem> {
        /// <summary>
        ///     Внутренний список <see cref="DataItem"/>
        /// </summary>
        private readonly List<DataItem> _dataItems = new List<DataItem>();
        /// <summary>
        ///     Внутреннее перечисление коллизий
        /// </summary>
        private IEnumerable<DataItemLabelCollision> Collisions {
            get { return this.Select(_ => new DataItemLabelCollision(this, _, this.Where(_.IsCollision))).Where(_ => !_.IsEmpty); }
        }
        /// <summary>
        ///     Внутренее значение температуры колонки
        /// </summary>
        private decimal Temperature {
            get { return Collisions.Any() ? Collisions.Select(_ => _.Temperature).Sum()/Collisions.Count() : 0; }
        }
        /// <summary>
        ///     Массив <see cref="DataItem"/>, формирующих данную колонку в виде <see cref="DataItemColon"/>
        /// </summary>
        public DataItem[] Items {
            get { return _dataItems.ToArray(); }
            set {
                _dataItems.Clear();
                _dataItems.AddRange(value);
            }
        }
        /// <summary>
        ///     Колонка значений графика для расчета их позиций
        /// </summary>
        public DataItemColon() {}
        /// <summary>
        ///     Колонка значений графика для расчета их позиций
        /// </summary>
        /// <param name="dataItems">Перечисление <see cref="DataItem"/>, образующих колонку</param>
        public DataItemColon(IEnumerable<DataItem> dataItems) {
            _dataItems.AddRange(dataItems);
        }
        /// <summary>
        ///     Минимализация температуры колонки
        /// </summary>
        public void MinimizeTemperature() {
            this.SelectSimilar().ForEach(_ => _.Skip(1).ForEach(__ => __.HideLabel()));
            EnsureBestLabels();
            Collisions.SelectMany(_ => _.SelectSimilar()).ForEach(_ => _.HideLabel());
            Collisions.SelectMany(_ => _.SelectVeryHot()).ForEach(_ => _.HideLabel());
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по <see cref="DataItem"/></returns>
        public IEnumerator<DataItem> GetEnumerator() {
            return _dataItems.GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator"/> по <see cref="DataItem"/>
        /// </summary>
        /// <returns><see cref="IEnumerator"/> по <see cref="DataItem"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        /// <summary>
        ///     Убеждается в наилучшем расположении «лычек»
        /// </summary>
        private void EnsureBestLabels() {
            if (Temperature == 0) {
                return;
            }

            var labelPositions = BuildBestLabelPositionsVariant();
            Apply(labelPositions);
        }
        /// <summary>
        ///     Собирает наилучший вариант расположения лычек чарта
        /// </summary>
        /// <returns>Выбранный вариант расположения лэйблов</returns>
        private LabelPosition[] BuildBestLabelPositionsVariant() {
            return new LabelPositionVariants(this).Select(_ => new KeyValuePair<decimal, LabelPosition[]>(Apply(_), _)).OrderBy(_ => _.Key).FirstOrDefault().Value;
        }
        /// <summary>
        ///     Последовательно применяет переданный массив <see cref="LabelPosition"/> к данной колонке
        /// </summary>
        /// <param name="labelPositions">Массив <see cref="LabelPosition"/></param>
        /// <returns>Температура колонки после применения переданного массива позиций лэйблов</returns>
        private decimal Apply(LabelPosition[] labelPositions) {
            var items = this.ToArray();
            for (var i = 0; i < labelPositions.Length; i++) {
                items[i].LabelPosition = labelPositions[i];
            }
            return Temperature;
        }
    }
}