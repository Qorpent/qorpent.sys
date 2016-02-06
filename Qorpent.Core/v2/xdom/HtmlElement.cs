using System.Linq;
using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public class HtmlElement : DomElement {
        public HtmlElement(params object[] content) :base(content, GetStructure()) { }
        public HtmlElement Html => this;
        public HeadElement Head => Elements("head").FirstOrDefault() as HeadElement;
        public BodyElement Body => Elements("body").FirstOrDefault() as BodyElement;
        private static object[] GetStructure() {
            return new object[] {
                new HeadElement(),
                new BodyElement(),  
            };
        }

        public XDocument ToDocument()
        {
            return new XDocument(new XDocumentType("html", null, null, null), this);
        }

        protected override bool SpecialSetTextContent(object item) {
            Body.Set(item);
            return true;
        }

        protected override bool SpecialSetElement(XElement e) {
            e = AddaptElement(e);
            if (e is HtmlElement) {
                ApplyToDomElement(e,this);
            }else if (e is HeadElement) {
                ApplyToDomElement(e,this.Head);
            }else if (e is BodyElement) {
                ApplyToDomElement(e,this.Body);
            }else if (e is InHeadElement) {
                Head.Set(e);
            }
            else {
                Body.Set(e);
            }
            return true;
        }
    }
}