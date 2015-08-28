using Qorpent;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.data.agg {
    public class CountCollector : Collector {
        public override object GetValue(object src, object currentValue, AggregateNode trg, IScope scope)
        {
            var result = InternalGetValue(src, currentValue, trg, scope);
            if (!Equals(0,result)) {
                var i = result.ToInt();
                if (0 == i) {
                    i = 1;
                }
                
                return currentValue.ToInt() + i;
            }
            return currentValue.ToInt();
        }
    }
}