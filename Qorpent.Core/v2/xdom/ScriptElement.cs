using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom {
    public class ScriptElement : InHeadElement
    {
        public ScriptElement(params object[] content) : base(content, flags:DomElementFlags.PreferHead|DomElementFlags.AllowText|DomElementFlags.RequireValue) {
            if (string.IsNullOrWhiteSpace(Type)) {
                this.Type = "text/javascript";
            }
        }

        public string Type {
            get { return this.Attr("type"); }
            set { SetAttributeValue("type", value); }
        } 
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

        public string DataMain
        {
            get { return this.Attr("data-main"); }
            set { SetAttributeValue("data-main", value); }
        }

        public static ScriptElement RequireJs(string main = null, string requireJs = null) {
            if (string.IsNullOrWhiteSpace(requireJs)) {
                requireJs = "scripts/require.js";
            }
            if (string.IsNullOrWhiteSpace(main)) {
                main = "scripts/main.js";
            }
            var result = new ScriptElement {DataMain = main, Src = requireJs};
            return result;
        }

        public static ScriptElement Script(string srcOrCode) {
            var result = new ScriptElement();
            if (srcOrCode.Contains("{") || srcOrCode.Contains("[")) {
                result.Value = srcOrCode;
            }
            else {
                result.Src = srcOrCode;
            }
            return result;
        }

    }
}