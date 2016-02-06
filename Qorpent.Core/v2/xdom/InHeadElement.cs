using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public abstract class InHeadElement : DomElement {
     
    

        public HeadElement Head => Parent as HeadElement;
        public HtmlElement Html => Head?.Html;

        protected InHeadElement(XElement other) : base(other) {
        }

        protected InHeadElement(object[] content, object[] structure = null, string name = null, DomElementFlags flags = DomElementFlags.Default) : base(content, structure, name, flags | DomElementFlags.PreferHead) {
        }
    }
}