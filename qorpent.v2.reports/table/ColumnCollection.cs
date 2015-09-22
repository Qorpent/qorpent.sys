using System;
using System.Collections.Generic;
using System.Linq;
using qorpent.v2.data.agg;

namespace qorpent.v2.reports.table {
    public class ColumnCollection {
        public ColumnCollection() {
            
        }
        public ColumnCollection(IEnumerable<IColumnDescriptor> columns) {
            Register(columns);
        }
        IDictionary<string, IColumnDescriptor> colgroups = new Dictionary<string, IColumnDescriptor>();
        public void Register(IEnumerable<IColumnDescriptor> columns)
        {
           
            var idx = 0;
            foreach (var c in columns)
            {
                if (c.Index == 0)
                {
                    c.Index = (idx += 10);
                }
                if (string.IsNullOrWhiteSpace(c.Group))
                {
                    c.Group = Guid.NewGuid().ToString();
                }
                Register(c);
            }



        }

        public IEnumerable<IEnumerable<IColumnDescriptor>> GetRows() {
            for (var i = 0; i < Rows; i++) {
                yield return GetRow(i);
            }
        } 

        public IEnumerable<IColumnDescriptor> GetRow(int level = 0) {
            if (level >= Rows || level<0) {
                throw new IndexOutOfRangeException(nameof(level));
            }
            foreach (var group in colgroups.Values.OrderBy(_=>_.Index)) {
                if (level == 0) {
                    if (1 == group.SubColumns.Count) {
                        group.SubColumns[0].RowSpan = group.RowSpan;
                        yield return group.SubColumns[0];
                    }
                    else {
                        yield return group;
                    }

                }else if (level == 1) {
                    if (1 < group.SubColumns.Count) {
                        foreach (var column in group.SubColumns) {
                            yield return column;
                        }
                    }
                }
            }
        } 

        public int Rows { get; private set; } = 1;

        public void Register(IColumnDescriptor c) {
            if (!colgroups.ContainsKey(c.Group)) {
                var group = new Collector {
                    Group = c.Group,
                    Name = c.GroupName,
                    SubColumns = new List<IColumnDescriptor>(),
                    Index = c.Index,
                    RowSpan = 1,
                    ColSpan = 1
                };
                colgroups[c.Group] = @group;
            }
            var t = colgroups[c.Group];
            t.SubColumns.Add(c);
            t.ColSpan = t.SubColumns.Count;
            if (t.ColSpan != 1) {
                Rows = 2;
                t.RowSpan = 1;
                foreach (var _c in colgroups.Values) {
                    if (_c.ColSpan == 1) {
                        _c.RowSpan = 2;
                    }
                }
                foreach (var sc in t.SubColumns) {
                    if (!string.IsNullOrWhiteSpace(sc.GrouppedName)) {
                        sc.ShortName = sc.GrouppedName;
                    }
                }
            }
            else {
                t.RowSpan = Rows;
            }
        }
    }
}