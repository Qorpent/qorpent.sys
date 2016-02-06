using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public class TrElement : ContentElement
    {
        public TrElement(params object[] content) : base(content) { }
        public TablePartElement Part => Parent as TablePartElement;
        public TableElement Table => Part?.Table;
        public IEnumerable<TableCellElement> Cells() => Elements().OfType<TableCellElement>();
        protected override bool SpecialSetTextContent(object item) {
            TableCellElement c;
            if (this.Part is TheadElement)
            {
                c = new ThElement();
            }
            else {
                c = new TdElement();
            }
            this.Add(c);
            c.Set(item);
            return true;
        }

        protected override bool SpecialSetElement(XElement e) {
            if (e is TableCellElement) {
                this.Add(e);
            }
            else {
                TableCellElement c;
                if (this.Part is TheadElement) {
                    c = new ThElement();
                }
                else {
                    c = new TdElement();
                }
                this.Add(c);
                c.Set(e);
            }
            return true;
        }
    }
}