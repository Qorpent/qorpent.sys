using System;
using System.Collections.Generic;
using Qorpent;

namespace qorpent.v2.data.agg {
    public interface IRouter {
        string Key { get; set; }
        IRouter Parent { get; set; }
        IList<IRouter> Children { get; }
        bool HasChildren { get; }
        int Level { get; }
        string ShortName { get; set; }
        string Name { get; set; }
        object Custom { get; set; }
        RouteKey GetKey(object obj, IScope scope);
        bool Filter(object o, AggregateNode current, IScope scope);
    }
}