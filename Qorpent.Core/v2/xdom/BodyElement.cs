using qorpent.v2.xdom;

namespace qorpent.v2.xdom {
    public class BodyElement : DomElement
    {
        public BodyElement(params object[] content) : base(content) { }
        public HtmlElement Html => Parent as HtmlElement;
    }
}