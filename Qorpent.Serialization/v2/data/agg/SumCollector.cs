using Qorpent;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.data.agg {
    public class SumCollector : Collector {
        public override object GetValue(object src, object currentValue, AggregateNode trg, IScope scope) {
            var result = InternalGetValue(src, currentValue, trg, scope);
            if (null != result) {
                var dec = result.ToDecimal();
                var current = currentValue.ToDecimal();
                return current + dec;
            }
            return currentValue;
        }
    }
}