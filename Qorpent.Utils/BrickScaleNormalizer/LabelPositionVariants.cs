using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Представление набора вариантов для <see cref="DataItemColon"/>
    /// </summary>
    internal class LabelPositionVariants : IEnumerable<LabelPosition[]> {
        /// <summary>
        ///     Количество элементов в <see cref="DataItemColon"/>
        /// </summary>
        private readonly DataItemColon _colon;
        /// <summary>
        ///     Представление набора вариантов для <see cref="DataItemColon"/>
        /// </summary>
        /// <param name="colon">Экземпляр <see cref="DataItemColon"/></param>
        public LabelPositionVariants(DataItemColon colon) {
            _colon = colon;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LabelPosition[]> Get() {
            var values = new List<string>();
            var v = _colon.Count();
            for (var ch = 'a'; ch <= 'c'; ch++) values.Add(ch.ToString(CultureInfo.InvariantCulture));

            for (var i = 1; i < v; i++) {
                var newValues = new List<string>();
                values.DoForEach(_ => { for (var ch = 'a'; ch <= 'c'; ch++) newValues.Add(_ + ch); });
                values = newValues;
            }

            var indexes = GetHiddenIndexes().ToList();

            foreach (var el in values) {
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
                indexes.ForEach(_ => c[_] = LabelPosition.Hidden);
                yield return c;
            }
        }
        private IEnumerable<int> GetHiddenIndexes() {
            var itemNum = -1;
            var hidden = _colon.Select(_ => new KeyValuePair<int, LabelPosition>(++itemNum, _.LabelPosition));
            return hidden.Where(_ => _.Value == LabelPosition.Hidden).Select(_ => _.Key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LabelPosition[]> GetEnumerator() {
            return Get().GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}