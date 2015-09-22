using System.Collections.Generic;
using System.Xml.Linq;

namespace qorpent.v2.reports.table {
    public interface IColumnDescriptor
    {

        string Name { get; set; }
        string ShortName { get; set; }
        string Group { get; set; }
        string GroupName { get; set; }
        object Custom { get; set; }
        int Index { get; set; }
        IList<IColumnDescriptor> SubColumns { get; set; }
        void SetupCell(XElement cell, object val);
   
        int ColSpan { get; set; }
        int RowSpan { get; set; }
        string GrouppedName { get; set; }
    }
}