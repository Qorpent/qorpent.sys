using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public abstract class NonContentElement : InBodyElement
    {
        protected NonContentElement(XElement other) : base(other) {
        }

        protected NonContentElement(object[] content, object[] structure = null, string name = null, DomElementFlags flags = DomElementFlags.None) : base(content, structure, name, flags ) {
        }

        
    }
}