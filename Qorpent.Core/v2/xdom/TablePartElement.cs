using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public abstract class TablePartElement : ContentElement {
        public TableElement Table => this.Parent as TableElement;
        protected TablePartElement(params object[] content) : base(content, flags: DomElementFlags.AllowElements) { }
        protected override bool SpecialSetTextContent(object item) {
            GetLastRow().Set(item);
            return true;
        }

        public TablePartElement AppendCells(params object[] items) {
            var r = GetLastRow();
            foreach (var item in items) {
                r.Set(item);
            }
            return this;
        }

        public TablePartElement AddRow(params object[] items) {
            var r = new TrElement();
            this.Set(r);
            foreach (var item in items) {
                r.Set(item);
            }
            return this;
        }

        protected override bool SpecialSetElement(XElement e) {

            e = AddaptElement(e);
            if (e is TrElement) {
                this.Add(e);
            }
            else  {
                var r = GetLastRow();
                r.Set(e);
            }
           

            return true;
        }

        public TrElement GetLastRow() {
            var r = Rows().LastOrDefault();
            if (null == r) {
                r = new TrElement().appendTo(this);
            }
            return r;
        }

        public IEnumerable<TrElement> Rows() {
            return Elements().OfType<TrElement>();
        } 
    }
}