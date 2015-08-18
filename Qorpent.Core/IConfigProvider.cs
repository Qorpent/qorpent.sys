using System.Xml.Linq;
using Qorpent.BSharp;

namespace Qorpent {
    public interface IConfigProvider {
        XElement GetConfig();
        IBSharpContext GetContext();
    }
}