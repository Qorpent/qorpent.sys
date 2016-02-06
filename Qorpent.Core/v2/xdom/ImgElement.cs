using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom {
    public class ImgElement : NonContentElement
    {
        public ImgElement(params object[] content) : base(content) { }
        public string Src
        {
            get { return this.Attr("src"); }
            set { SetAttributeValue("src", value); }
        }
        public string Href
        {
            get { return Src; }
            set { Src = value; }
        }
    }
}