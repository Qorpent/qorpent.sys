using System.Xml.Linq;

namespace Qorpent.Utils {
    public interface IXmlIncludeProvider {
        XElement GetXml(string path, XElement current, IScope scope);
    }
}