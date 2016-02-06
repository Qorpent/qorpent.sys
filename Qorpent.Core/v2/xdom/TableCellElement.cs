using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom {
    public abstract class TableCellElement : ContentElement  {
        protected TableCellElement(params object[] content) : base(content, flags: DomElementFlags.Default) { }
        public int ColSpan
        {
            get { return this.Attr("colspan").ToInt(); }
            set { SetAttributeValue("colspan", value); }
        }
        public int RowSpan
        {
            get { return this.Attr("rowspan").ToInt(); }
            set { SetAttributeValue("rowspan", value); }
        }
        public TrElement Row => this.Parent as TrElement;
        public TablePartElement Part => Row?.Part;
        public TableElement Table => Part?.Table;

        
    }
}