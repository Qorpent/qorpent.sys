using System.IO;
using System.Xml.Linq;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;
using Qorpent.Bxl;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.renders {
    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IReportRender),Name="qorpent.reports.xi.template")]
    public class XmlInterpolationReportRender : ReportRenderBase {
        private XElement Template;
        private XmlInterpolation Xi;
        
        public override IScope Render(IReportContext context, IScope scope, object item) {
            scope = scope ?? context.Scope;
            Template = Template ?? LoadTemplate();

            Xi = Xi ?? new XmlInterpolation() { UseExtensions = true };
            var ws = new Scope();
            ws["data"] = context.Data;
            ws["context"] = context;
            ws["scope"] = scope;
            ws["item"] = item;
            ws["items"] = context.Data.arr("items");
            var result = Xi.Interpolate(Template, ws);
            foreach (var element in result.DescendantsAndSelf()) {
                element.SetAttributeValue("_file",null);
                element.SetAttributeValue("_line",null);
            }
            if (scope.Get("store_render").ToBool()) {
                scope[scope.Get("render_name", "render_result")] = result;
            }
            if (!scope.Get("no_render").ToBool()) {
                context.Write(result.ToString());
            }
            return scope;
        }

        private XElement LoadTemplate() {
            if (FileName.EndsWith(".bxl")) {
                return new BxlParser().Parse(File.ReadAllText(FileName), FileName, BxlParserOptions.ExtractSingle);
            }
            return XElement.Load(FileName);
        }
    }
}