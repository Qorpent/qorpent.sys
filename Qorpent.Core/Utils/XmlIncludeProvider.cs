using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using qorpent.v2.reports.storage;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    
    public class XmlIncludeProvider : IXmlIncludeProvider {
        private IDictionary<string, IXmlComponent> _componentCache;

        public XmlIncludeProvider() {
            IncludeCache = new Dictionary<string, XElement>();
        }
        IDictionary<string, XElement> IncludeCache { get; set; }
        public IRenderProvider RenderProvider { get; set; }

        public IContainer Container { get; set; }

        public XElement GetXml(string path, XElement current, IScope scope) {
            if (path.Contains("component://")) {
                return ResolveRenderComponent(path,current,scope);
            }
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

        public IDictionary<string, IXmlComponent> ComponentCache
        {
            get { return _componentCache ?? (_componentCache=new Dictionary<string, IXmlComponent>()); }
        }


        private XElement ResolveRenderComponent(string path, XElement current, IScope scope) {
            
            var uri = new Uri(path);
            var name = (uri.Host + "." + uri.AbsolutePath).Replace("/", ".").Replace("..", ".");
            if (name.EndsWith(".")) {
                name = name.Substring(0, name.Length - 1);
            }
            IXmlComponent component = null;
            if (ComponentCache.ContainsKey(name)) {
                component = ComponentCache[name];
            }
            else {
                if (null == Container) {
                    throw new Exception("components can be used only with Container setup " + path);
                }
                component = Container.Get<IXmlComponent>(name);
            }
            var componentResult = component.Create(path, current, scope);
            return componentResult;
        }


        public virtual void Preprocess(XElement result,XElement current,IScope scope) {
          
        }
    }
}