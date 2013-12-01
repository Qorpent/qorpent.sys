using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    /// Колонка значений графика для расчета их позиций
    /// </summary>
    public class DataItemColon : IEnumerable<DataItem> {
        /// <summary>
        ///     Внутренний списко <see cref="DataItem"/>
        /// </summary>
        private readonly List<DataItem> _dataItems = new List<DataItem>();
        /// <summary>
        /// 
        /// </summary>
        public decimal ScaleMax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataItemColon() {}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataItems"></param>
        public DataItemColon(IEnumerable<DataItem> dataItems) {
            _dataItems.AddRange(dataItems);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemLabelCollision> FindCollisions() {
            DataItem previous = null;
            DataItem current = null;
            for (var i = 0; i < Items.Length; i++) {
                if(Items[i].LabelPosition==LabelPosition.Hidden)continue;
                previous = current;
                current = Items[i];
                if (0 == i && current.NormalizedLabelMin < 0) {
                    yield return new DataItemLabelCollision{ScaleMax = ScaleMax,First = current,Second = null};
                }
                if (null != previous && (previous.NormalizedLabelMax > current.NormalizedLabelMin) &&
                    (current.NormalizedValue > previous.NormalizedLabelMax)) {
                    yield return new DataItemLabelCollision { ScaleMax = ScaleMax, First = previous, Second = current };
                }
                if (null != previous && (current.NormalizedLabelMax > previous.NormalizedLabelMin) &&
                    (previous.NormalizedValue >  current.NormalizedLabelMax))
                {
                    yield return new DataItemLabelCollision { ScaleMax = ScaleMax, First = current, Second = previous };
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DataItem[] Items {
            get { return _dataItems.ToArray(); }
            set {
                _dataItems.Clear();
                _dataItems.AddRange(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal GetTemperature() {
            return FindCollisions().Select(_ => _.Overlap).Sum();
        }

        /// <summary>
        /// 
        /// </summary>
        public void TryMinimizeTemperature() {
            if (GetTemperature() == 0) return;
            ResetTemperature();
            int variant = 0;
            decimal currentMin = GetTemperature();
            var keepTopAndBottom = KeepTopAndBottom();
            if (keepTopAndBottom < currentMin)
            {
                variant = 1;
                currentMin = keepTopAndBottom;
            }
            ResetTemperature();
            var fromdownVariant = ColdFromDown();
            if (fromdownVariant < currentMin) {
                variant = 2;
                currentMin = fromdownVariant;
            }
            ResetTemperature();
            var fromUpVariant = ColdFromUp();
            if (fromUpVariant < currentMin)
            {
                variant = 3;
                currentMin = fromUpVariant;
            }
            ResetTemperature();
            var threeWorst = ColdFromThreeWorst();
            if (threeWorst < currentMin)
            {
                variant = 4;
            }
            if (0 == variant) {
                foreach (var dataItem in Items) {
                    dataItem.LabelPosition = LabelPosition.Auto;
                }
            }
            else if (1 == variant)
            {
                KeepTopAndBottom();
            }
            else if(2==variant) {
                ColdFromDown();
            }else if (3 == variant) {
                ColdFromUp();
            }
            else {
                ColdFromThreeWorst();
            }
        }

        private void ResetTemperature() {
            foreach (var dataItem in Items) {
                dataItem.LabelPosition = LabelPosition.Auto;
            }
        }

        private decimal KeepTopAndBottom() {
            var collisions = FindCollisions().ToArray();
            var fst = collisions.FirstOrDefault(_ => _.Second == null);
            var sec = collisions.FirstOrDefault(_ => _.First == null);
            if (null != fst) {
                fst.First.LabelPosition = LabelPosition.Above;
            }
            if (null != sec) {
                sec.Second.LabelPosition = LabelPosition.Below;
            }
            collisions = FindCollisions().Where(_ => null != _.First && null != _.Second ).ToArray();
            foreach (var collision in collisions) {
                if (collision.First == fst.First && collision.Second == sec.Second) {
                    continue;
                }
                if (collision.First == fst.First) {
                    collision.Second.LabelPosition = LabelPosition.Above;
                }
                else if (collision.Second == sec.Second) {
                    collision.First.LabelPosition = LabelPosition.Below;
                }
                else {
                    collision.Second.LabelPosition= LabelPosition.Above;
                }
            }

            return GetTemperature();
        }

        private decimal ColdFromThreeWorst() {
            for (var i = 0; i < 3; i++) {
                var collision = FindCollisions().OrderByDescending(_ => _.Overlap).FirstOrDefault();
                if (null == collision) break;
                if (collision.Second == null) {
                    collision.First.LabelPosition = LabelPosition.Above;
                }else if (collision.First == null) {
                    collision.Second.LabelPosition = LabelPosition.Below;
                }
                else {
                    collision.Second.LabelPosition = LabelPosition.Above;
                    collision.First.LabelPosition = LabelPosition.Below;
                }
            }
            return GetTemperature();
        }

        private decimal ColdFromUp() {
            throw new NotImplementedException();
        }

        private decimal ColdFromDown() {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DataItem> GetEnumerator() {
            return _dataItems.GetEnumerator();
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