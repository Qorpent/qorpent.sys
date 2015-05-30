using System.Xml.Linq;

namespace Qorpent {
    public interface IConfigProvider {
        XElement GetConfig();
    }
}