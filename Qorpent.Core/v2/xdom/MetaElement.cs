namespace qorpent.v2.xdom {
    public class MetaElement : InHeadElement
    {
        public MetaElement(params object[] content) : base(content, flags:DomElementFlags.None) { }
    }
}