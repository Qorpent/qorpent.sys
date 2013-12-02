using System;
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
            EnsureBestLabels();
            Collisions.SelectMany(_ => _.SelectVeryHot()).ForEach(_ => _.HideLabel()); // дошлейфовка
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
            ApplyLabelPositions(labelPositions);
        }
        /// <summary>
        ///     Собирает наилучший вариант расположения лычек чарта
        /// </summary>
        /// <returns></returns>
        private LabelPosition[] BuildBestLabelPositionsVariant() {
            var totalizator = new List<KeyValuePair<decimal, LabelPosition[]>>();
            BuildVariants().DoForEach(_ => {
                ApplyLabelPositions(_);
                totalizator.Add(new KeyValuePair<decimal, LabelPosition[]>(Temperature, _));
            });
            return totalizator.OrderBy(_ => _.Key).FirstOrDefault().Value;
        }
        /// <summary>
        ///     Собирает разброс вариантов <see cref="LabelPosition"/>
        /// </summary>
        /// <returns>Разброс вариантов <see cref="LabelPosition"/></returns>
        private IEnumerable<LabelPosition[]> BuildVariants() {
            var v = this.Count();
            var list = GenerateLetterCombinations(v);

            foreach (var el in list) {
                var c = new LabelPosition[v];
                var k = 0;
                foreach (var ch in el.ToCharArray()) {
                    switch (ch) {
                        case 'a': c[k] = LabelPosition.Auto; break;
                        case 'b': c[k] = LabelPosition.Above; break;
                        case 'c': c[k] = LabelPosition.Below; break;
                    }
                    k++;
                }
                yield return c;
            }
        }

        private List<string> GenerateLetterCombinations(int numLetters) {
            var values = new List<string>();

            for (var ch = 'a'; ch <= 'c'; ch++) {
                values.Add(ch.ToString());
            }

            for (var i = 1; i < numLetters; i++) {
                var newValues = new List<string>();
                foreach (var str in values) {
                    for (var ch = 'a'; ch <= 'c'; ch++) {
                        newValues.Add(str + ch);
                    }
                }

                values = newValues;
            }

            return values;
        }

        /// <summary>
        ///     Последовательно применяет переданный массив <see cref="LabelPosition"/> к данной колонке
        /// </summary>
        /// <param name="labelPositions">Массив <see cref="LabelPosition"/></param>
        private void ApplyLabelPositions(LabelPosition[] labelPositions) {
            if (labelPositions.Length != this.Count()) {
                throw new Exception("LabelPosition array length mismatch");
            }
            var items = this.ToArray();
            for (var i = 0; i < labelPositions.Length; i++) {
                items[i].LabelPosition = labelPositions[i];
            }
        }
    }
}