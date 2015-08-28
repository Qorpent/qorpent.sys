using System;
using Qorpent;

namespace qorpent.v2.data.agg {
    public class Collector :ICollector{
        private static int ID = 0;
        public Collector()
        {
            Id = ID++;
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Func<object,object , AggregateNode, IScope,object> ValueFunction { get; set; }
        public virtual object GetValue(object src, object currentValue, AggregateNode trg, IScope scope) {
            var result = InternalGetValue(src, currentValue, trg, scope);
            return result ?? currentValue;
        }

        public virtual bool Filter(object src, AggregateNode trg, IScope scope) {
            return true;
        }

        protected virtual object InternalGetValue(object src, object currentValue, AggregateNode trg, IScope scope) {
            if (null != ValueFunction) {
                {
                    return ValueFunction(src, currentValue, trg, scope);
                }
            }
            return null;
        }
    }
}