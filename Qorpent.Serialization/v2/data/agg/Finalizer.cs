using System;

namespace qorpent.v2.data.agg {
    public class Finalizer:IFinalizer {
        public Action<AggregateNode> ExecuteFunction { get; set; }

        public virtual void Execute(AggregateNode node) {
            if (null != ExecuteFunction) {
                ExecuteFunction(node);
            }
        }
    }
}