using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;
using Qorpent.Experiments;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder.Tasks.json {
    public abstract class JsonGeneratorTaskBase: BSharpBuilderTaskBase
    {
        public JsonGeneratorTaskBase()
        {
            Phase = BSharpBuilderPhase.PostProcess;
            Index = TaskConstants.WriteWorkingOutputTaskIndex + 100 + 10;
        }

        public void WriteJson(IBSharpClass cls, object json = null, IList<string> opts =null) {
            json = json ?? new {};
            var path = Path.Combine(Project.RootDirectory, Project.Definition.Attr("JsonDir"), cls.Prototype,
                cls.Name + "." + cls.Prototype + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var j = json is IDictionary<string,object>? json as IDictionary<string,object>: json.jsonifymap();
            Refine(j, cls.Compiled,_opts:opts);
            File.WriteAllText(path,j.stringify(pretty:true));
        }
        public string[] ExcludeAttributes { get; set; }
        
        protected IDictionary<string, object> GetOptions(XElement xml, IList<string> _opts, params string[] excludeAttrs) {
            var result = new Dictionary<string,object>();
            var guess = !_opts.Contains("noguess");
            
            foreach (var attribute in xml.Attributes()) {
                if(attribute.Name.LocalName=="id")continue;
                if(attribute.Name.LocalName=="code")continue;
                if(attribute.Name.LocalName=="name")continue;
                if(attribute.Name.LocalName=="_file")continue;
                if(attribute.Name.LocalName=="_line")continue;
                
                if (-1!=Array.IndexOf(excludeAttrs,attribute.Name.LocalName))continue;
                if (null!=ExcludeAttributes && -1!=Array.IndexOf(ExcludeAttributes, attribute.Name.LocalName))continue;
                result[attribute.Name.LocalName] = guess ? attribute.Value.Guess() : attribute.Value;
            }
            return result;
        }
        
        protected IDictionary<string, object> Refine(
            object target, 
            XElement src, 
            bool removezeroes = true, 
            string[] remove = null,
            string[] noopt = null ,
            bool allnooptions = false,
            IList<string> _opts = null 
            ) {
            _opts = _opts ?? _emptyopts;
            var guess = !_opts.Contains("noguess");
            if (null == src) {
                return target.jsonifymap();
            }
            remove = remove ?? new string[] {};
            noopt = noopt ?? new string[] {};
            var opts =
                new[] {"id", "code", "name", "fullcode", "prototype"}.Union(noopt).Union(remove).ToArray();
            var options = GetOptions(src, _opts,opts);
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
            foreach (var rootattribute in noopt)
            {
                var val = src.Attr(rootattribute);
                object _val = val;
                if (_opts.Contains("parse-arrays") && val.StartsWith("["))
                {
                    _val = val.Substring(1, val.Length - 1).SmartSplit(false, true, ';').Select(_ =>guess? _.Guess():_).ToArray();
                }
                j[rootattribute] = guess? _val.Guess():_val;
            }
           
            if (removezeroes) {
                RemoveZeroes(options);
                RemoveZeroes(j);
            }
            if (options.Count != 0)
            {
                if (allnooptions) {
                    foreach (var option in options) {
                        j[option.Key] = option.Value;
                    }

                }
                else {
                    j["options"] = options;
                }
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

        protected void WriteElements(XElement xml, IDictionary<string, object> j, IList<string> opts  )
        {
            var names = xml.Elements().Select(_ => _.Name.LocalName).Distinct();
            foreach(var n in names)
            {
                
                var els = xml.Elements(n).ToArray();
                var set = new List<object>();
                foreach(var e in els)
                {
                    set.Add(ConvertElement(e,opts));
                }


                var normalname = n + (n.EndsWith("s") ? "es" : "s");
                while (j.ContainsKey(normalname))
                {
                    normalname += "_";
                }
                j[normalname] = set.ToArray();
            }
        }
        static List<string> _emptyopts =new List<string>(); 
        protected IDictionary<string, object> ConvertElement(XElement e, IList<string> opts) {
            opts = opts ?? _emptyopts;
            var guess = !opts.Contains("noguess");
            var result = new Dictionary<string, object>();
            foreach(var a in e.Attributes().OrderBy(_=>_.Name.LocalName)) {
                var name = a.Name.LocalName;
                if (name == "code" && null == e.Attribute("id")) {
                    name = "id";
                }
                var val = a.Value;
                object _val = val;
                if (opts.Contains("parse-arrays") && val.StartsWith("[")) {
                    _val = val.Substring(1, val.Length - 1).SmartSplit(false, true, ';').Select(_=>guess?_.Guess():_).ToArray();
                }
                result[name] =guess?_val.Guess():_val;
            }
            WriteElements(e, result,opts);
            return result;
        }
    }
}