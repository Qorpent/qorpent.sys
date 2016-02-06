using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public class HeadElement : DomElement {
        public HeadElement(params object[] content) :base(content, GetStructure()) { }
        public HtmlElement Html => Parent as HtmlElement;

        private static object[] GetStructure()
        {
            return new object[] {
                new MetaElement(new XAttribute("charset","utf-8")),
            };
        }
    }

    
}