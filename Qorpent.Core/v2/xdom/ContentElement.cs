using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public abstract class ContentElement : InBodyElement
    {
        protected ContentElement(XElement other) : base(other) {
        }

        protected ContentElement(object[] content, object[] structure = null, string name = null, DomElementFlags flags = DomElementFlags.Default) : base(content, structure, name, flags) {
        }
    }
}