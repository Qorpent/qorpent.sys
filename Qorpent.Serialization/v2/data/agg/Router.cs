using System;
using System.Collections.Generic;
using Qorpent;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.data.agg {
    public class Router:IRouter
    {
        private static int ID = 0;
        public Router()
        {
            Id = ID++;
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public object Custom { get; set; }

        public int Level
        {
            get { return null == Parent ? 0 : Parent.Level + 1; }
        }

        private IList<IRouter> _subRetrievers;

        public IRouter Parent { get; set; }

        public IList<IRouter> Children
        {
            get { return _subRetrievers ?? (_subRetrievers = new List<IRouter>()); }
        }


        public Router(Func<object, IScope, RouteKey> retriever)
        {
            Retriever = retriever;
        }
        public Func<object, IScope, RouteKey> Retriever { get; set; }
        public bool HasChildren { get { return null != _subRetrievers && 0 != _subRetrievers.Count; } }

        public virtual RouteKey GetKey(object obj, IScope scope)
        {
            if (null != Retriever)
            {
                return Retriever(obj, scope);
            }
            return new RouteKey {Name = obj.ToStr()};
        }

        public Func<object,AggregateNode,IScope,bool> FilterFunc { get; set; } 

        public virtual bool Filter(object o, AggregateNode current, IScope scope) {
            if (null != FilterFunc) {
                return FilterFunc(o, current, scope);
            }
            return true;
        }
    }
}