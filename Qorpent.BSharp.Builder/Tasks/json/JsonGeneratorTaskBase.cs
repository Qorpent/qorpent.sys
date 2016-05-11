using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Experiments;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder.Tasks.json {
    public abstract class JsonGeneratorTaskBase: BSharpBuilderTaskBase
    {
        public JsonGeneratorTaskBase()
        {
            Phase = BSharpBuilderPhase.PostProcess;
            Index = TaskConstants.WriteWorkingOutputTaskIndex + 100 + 10;
        }

        public void WriteJson(IBSharpClass cls, object json = null) {
            json = json ?? new {};
            var path = Path.Combine(Project.RootDirectory, Project.Definition.Attr("JsonDir"), cls.Prototype,
                cls.Name + "." + cls.Prototype + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var j = json is IDictionary<string,object>? json as IDictionary<string,object>: json.jsonifymap();
            Refine(j, cls.Compiled);
            File.WriteAllText(path,j.stringify(pretty:true));
        }
        public string[] ExcludeAttributes { get; set; }
        
        protected IDictionary<string, object> GetOptions(XElement xml, params string[] excludeAttrs) {
            var result = new Dictionary<string,object>();
            foreach (var attribute in xml.Attributes()) {
                if(attribute.Name.LocalName=="id")continue;
                if(attribute.Name.LocalName=="code")continue;
                if(attribute.Name.LocalName=="name")continue;
                if(attribute.Name.LocalName=="_file")continue;
                if(attribute.Name.LocalName=="_line")continue;
                
                if (-1!=Array.IndexOf(excludeAttrs,attribute.Name.LocalName))continue;
                if (null!=ExcludeAttributes && -1!=Array.IndexOf(ExcludeAttributes, attribute.Name.LocalName))continue;
                result[attribute.Name.LocalName] = attribute.Value;
            }
            return result;
        }
        
        protected IDictionary<string, object> Refine(object target, XElement src, bool removezeroes = true) {
            var options = GetOptions(src,"id","code","name","fullcode","prototype");
            var j = target is IDictionary<string, object> ? target as IDictionary<string, object> : target.jsonifymap();
            if (string.IsNullOrWhiteSpace(j.str("id"))) {
                j["id"] = src.GetCode();
            }
            if (string.IsNullOrWhiteSpace(j.str("name")) && !string.IsNullOrWhiteSpace(src.Attr("name"))) {
                j["name"] = src.Attr("name");
            }
            var texts = src.Nodes().OfType<XText>().ToArray();
            var str = string.Join("\n", texts.Select(_=>_.Value));
            if (!string.IsNullOrWhiteSpace(str)) {
                j["_value"] = str;
            }
            foreach (var option in options.ToArray()) {
                if (j.ContainsKey(option.Key)) {
                    options.Remove(option.Key);
                }
            }
            if (options.Count != 0) {
                j["options"] = options;
            }
            if (removezeroes) {
                RemoveZeroes(j);
            }
            return j;
        }

        private void RemoveZeroes(IDictionary<string, object> dictionary) {
            foreach (var p in dictionary.ToArray()) {
                var val = p.Value;
                if (!val.ToBool()) {
                    dictionary.Remove(p.Key);
                    continue;
                }
                if (val is Array) {
                    if (((Array) val).Length == 0) {
                        dictionary.Remove(p.Key);
                    }
                    continue;
                }
                if (val is IDictionary<string, object>) {
                    RemoveZeroes((IDictionary<string,object>)val);
                }
            }
        }

        protected void WriteElements(XElement xml, IDictionary<string, object> j)
        {
            var names = xml.Elements().Select(_ => _.Name.LocalName).Distinct();
            foreach(var n in names)
            {
                
                var els = xml.Elements(n).ToArray();
                var set = new List<object>();
                foreach(var e in els)
                {
                    set.Add(ConvertElement(e));
                }


                var normalname = n + (n.EndsWith("s") ? "es" : "s");
                while (j.ContainsKey(normalname))
                {
                    normalname += "_";
                }
                j[normalname] = set.ToArray();
            }
        }

        protected IDictionary<string,object> ConvertElement(XElement e)
        {
            var result = new Dictionary<string, object>();
            foreach(var a in e.Attributes())
            {
                result[a.Name.LocalName] = a.Value;
            }
            WriteElements(e, result);
            return result;
        }
    }
}