using Qorpent;
using Qorpent.Model;

namespace qorpent.v2.data.agg {
    public interface ICollector {
        object GetValue(object src, object currentValue, AggregateNode trg, IScope scope);
        bool Filter(object src, AggregateNode trg, IScope scope);
        string Key { get; set; }
        string Name { get; set; }
        string ShortName { get; set; }
    }
}