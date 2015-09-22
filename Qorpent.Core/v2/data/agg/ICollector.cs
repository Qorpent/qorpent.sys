using System.Security.Cryptography.X509Certificates;
using qorpent.v2.reports.table;
using Qorpent;
using Qorpent.Model;

namespace qorpent.v2.data.agg {
    public interface ICollector:IColumnDescriptor {
        object GetValue(object src, object currentValue, AggregateNode trg, IScope scope);
        bool Filter(object src, AggregateNode trg, IScope scope);
        string Key { get; set; }
        string Condition { get; set; }
        bool DrillDown { get; set; }

    }
}