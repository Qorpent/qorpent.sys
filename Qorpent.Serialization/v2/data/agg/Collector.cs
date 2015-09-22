using System;
using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.v2.reports.table;
using Qorpent;

namespace qorpent.v2.data.agg {
    

    public class Collector :ICollector, IColumnDescriptor
    {
        private static int ID = 0;
        private string _shortName;
        private string _name;

        public Collector()
        {
            Id = ID++;
        }

        public int Id { get; set; }
        public string Key { get; set; }

        public string Name
        {
            get { return _name ??Key; }
            set { _name = value; }
        }

        public string ShortName
        {
            get { return _shortName ?? Name; }
            set { _shortName = value; }
        }

        public string Group { get; set; }
        public string GroupName { get; set; }
        public string Condition { get; set; }
        public virtual void SetupCell(XElement cell, object val) {
        }

        public IList<IColumnDescriptor> SubColumns { get; set; }
        public int Index { get; set; }

        public bool DrillDown { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string GrouppedName { get; set; }

        public object Custom { get; set; }
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