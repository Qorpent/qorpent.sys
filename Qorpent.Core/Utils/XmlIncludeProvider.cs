using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    public class XmlIncludeProvider : IXmlIncludeProvider {
        public XmlIncludeProvider() {
            IncludeCache = new Dictionary<string, XElement>();
        }
        IDictionary<string, XElement> IncludeCache { get; set; }

        public XElement GetXml(string path, XElement current, IScope scope) {

            var filename = path.Contains("@") ? EnvironmentInfo.ResolvePath(path) : path;
            if (!Path.IsPathRooted(filename))
            {
                var dir = Path.GetDirectoryName(current.Describe().File);
                filename = Path.Combine(dir, filename);
            }          
            if (!IncludeCache.ContainsKey(filename)) {
                var result = IncludeCache[filename] = XmlExtensions.Load(filename, BxlParserOptions.ExtractSingle);
                Preprocess(result,current,scope);
                return result;
            }
            return IncludeCache[filename];
        }

        public virtual void Preprocess(XElement result,XElement current,IScope scope) {
          
        }
    }
}