using System.Xml.Linq;

namespace qorpent.v2.xdom {
    public class StyleElement : InHeadElement
    {
        public StyleElement(params object[] content) : base(content,GetStructure()) {
            
        }

        private static object[] GetStructure() {
            return new object[] {
                new XAttribute("type","text/css"), 
            };
        }

       
    }
}