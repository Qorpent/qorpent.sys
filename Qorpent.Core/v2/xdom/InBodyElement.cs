using System.Linq;
using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public abstract class InBodyElement : DomElement {
        protected InBodyElement(XElement other) : base(other) {
        }

        protected InBodyElement(object[] content, object[] structure = null, string name = null, DomElementFlags flags = DomElementFlags.None) : base(content, structure, name, flags ) {
        }

        public HtmlElement Html => Ancestors().OfType<HtmlElement>().FirstOrDefault();
        public BodyElement DocumentBody => Ancestors().OfType<BodyElement>().FirstOrDefault();
    }
}