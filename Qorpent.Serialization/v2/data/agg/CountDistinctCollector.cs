using System.Collections.Generic;
using Qorpent;
using Qorpent.Scaffolding.Model;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.data.agg {
    public class CountDistinctCollector : Collector {
        public override object GetValue(object src, object currentValue, AggregateNode trg, IScope scope)
        {
            var result = InternalGetValue(src, currentValue, trg, scope);
            if (null == result) return currentValue;
            var key = trg.Id + "_" + this.Id;
            var val = result.ToStr();
            var vals = scope.Ensure(key, new List<string>());
            if (vals.Contains(val)) {
                return currentValue;
                
            }
            vals.Add(val);
            return currentValue.ToInt() + 1;
        }
    }
}