using System.Xml.Linq;

namespace Qorpent.Utils {
    public interface IXmlComponent {
        XElement Create(string uri, XElement current, IScope scope);
    }
}