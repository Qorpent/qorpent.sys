using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Experiments;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder.Tasks.json
{


    public class UniversalJsonBuilder : JsonGeneratorTaskBase
    {
        public UniversalJsonBuilder()
        {
            this.ExcludeAttributes = new[] { "generate-json"};
        }
        public override void Execute(IBSharpContext context)
        {
            foreach (var cls in context.ResolveAll("attr:generate-json"))
            {
                var xml = RefineForJson(new XElement(cls.Compiled));
                var opts = xml.Attr("generate-json").SmartSplit();
                IDictionary<string,object> j = new Dictionary<string, object>();

                if (opts.Contains("nooptions") && j.ContainsKey("options"))
                {
                    IDictionary<string, object> _opts = j["options"].jsonifymap();
                    j.Remove("options");
                    foreach(var o in _opts.OrderBy(_=>_.Key)) {
                        j[o.Key] = o.Value;
                    }
                }
                

                WriteElements(xml, j, opts);
                



                WriteJson(cls, j, opts);
            }
        }

        private XElement RefineForJson(XElement e)
        {
            var gjs = e.Elements("json-generate").ToArray();
            gjs.Remove();
            foreach(var gj in gjs)
            {
                ApplyRefineCommand(gj, e);
            }
            return e;
        }

        private void ApplyRefineCommand(XElement gj, XElement e)
        {
            var selectionxpath = gj.Attr("select");
            var selection = e.XPathSelectElements(selectionxpath).ToArray();
            if (0 != selection.Length)
            {
                if (gj.GetCode() == "rename-attribute")
                {
                    RenameAttribute(gj, e,selection);
                }
                else if (gj.GetCode() == "rename-element")
                {
                    RenameElement(gj, e,selection);
                }
            }
        }

        private void RenameElement(XElement gj, XElement e, XElement[] selection)
        {
            var targetattr = gj.Attr("targetattr");
            var newname = gj.Attr("newname");
            foreach(var s in selection)
            {
                if (!string.IsNullOrWhiteSpace(targetattr))
                {
                    s.SetAttributeValue(targetattr, s.Name.LocalName);
                }
                s.Name = newname;
            }
        }

        private void RenameAttribute(XElement gj, XElement e, XElement[] selection)
        {
            var oldname = gj.Attr("oldname");
            var newname = gj.Attr("newname");
            foreach(var s in selection)
            {
                var a = s.Attribute(oldname);
                if (null != a)
                {
                    a.Remove();
                    s.SetAttributeValue(newname, a.Value);
                }
            }
        }
    }
    
}
