using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom {
    public class LinkElement : InHeadElement
    {
        public LinkElement(params object[] content) : base(content, flags:DomElementFlags.AllowText) { }

        public string Rel {
            get { return this.Attr("rel"); }
            set { SetAttributeValue("rel",value);}
        }

        public string Type {
            get { return this.Attr("type"); }
            set {  SetAttributeValue("type",value);}
        }

        public string Href
        {
            get { return this.Attr("href"); }
            set { SetAttributeValue("href", value); }
        }



        public static LinkElement Stylesheet(string hrefOrCode) {
            var result = new LinkElement { Rel = "stylesheet", Type = "text/css" };
            if (hrefOrCode.Contains("{")) {
                result.Value = hrefOrCode;
            }
            else {
                result.Href = hrefOrCode;
            }
            return result;
        }
    }
}