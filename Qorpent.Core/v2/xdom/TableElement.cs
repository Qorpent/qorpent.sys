using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public class TableElement : ContentElement {
        public TableElement(params object[] content) : base(content,GetStructure(), flags:DomElementFlags.AllowElements) { }
        public TheadElement Head => Elements().OfType<TheadElement>().FirstOrDefault();
        public TbodyElement Body => Elements().OfType<TbodyElement>().FirstOrDefault();

        public TableElement AppendCells(params object[] items) {
            Body.AppendCells((object[]) items);
            return this;
        }

        public TableElement row(params object[] items)
        {
            Body.AddRow((object[])items);
            return this;
        }

        public TableElement rows(IEnumerable<IEnumerable> _rows)
        {
            foreach (var r in _rows)
            {
                row((object[]) r.OfType<object>().ToArray());
            }
            return this;
        }

        public TableElement AppendHeads(params object[] items)
        {
            Head.AppendCells((object[])items);
            return this;
        }

        public TableElement headrow(params object[] items)
        {
            Head.AddRow((object[])items);
            return this;
        }


        public IEnumerable<TrElement> Rows => Body?.Rows(); 
        public IEnumerable<TrElement> HeadRows => Head?.Rows(); 
        private static object[] GetStructure() {
            return new object[] {
                new TheadElement(),
                new TbodyElement()
            };
        }

        

        protected override bool SpecialSetTextContent(object item) {
            this.Body.Set(item);
            return true;
        }

        protected override bool SpecialSetElement(XElement e)
        {
            e = AddaptElement(e);
            if (e is TableElement)
            {
                ApplyToDomElement(e, this);
            }
            else if (e is TheadElement)
            {
                ApplyToDomElement(e, Head);
            }
            else if (e is TbodyElement)
            {
                ApplyToDomElement(e, Body);
            }
            else if (e is TablePartElement)
            {
                this.Add(e);
            }else if (e is ThElement) {
                this.Head.Set(e);
            }
            else if (e is TdElement) {
                this.Body.Set(e);
            }
            else {
                this.Body.Set(e);
            }
            return true;
        }
    }
}