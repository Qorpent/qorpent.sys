using System.IO;
using System.Xml.Linq;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using qorpent.v2.reports.storage;
using Qorpent;
using Qorpent.Bxl;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.renders {




    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IReportRender),Name="qorpent.reports.xi.template")]
    public class XmlInterpolationReportRender : ReportRenderBase, IContainerBound {
        private XElement Template;
        private XmlInterpolation Xi;
        private IContainer _container;


        public override IScope Render(IReportContext context, IScope scope, object item) {
            scope = scope ?? context.Scope;
            Template = Template ?? XmlExtensions.Load(FileName,BxlParserOptions.ExtractSingle);
            Xi = Xi ?? new XmlInterpolation { UseExtensions = true, XmlIncludeProvider = new XmlIncludeProvider {Container = _container} };
            IScope ws = null;
            if (null == context) {
                ws = scope;
            }
            else {
                ws = new Scope();
                ws["data"] = context.Data;
                ws["context"] = context;
                ws["scope"] = scope;
                ws["item"] = item;
                ws["items"] = context.Data.arr("items");
            }
            var result = Xi.Interpolate(Template, ws);
            RemoveDebugInfo(scope, result);
            Finalize(context, scope, result);
            return scope;
        }

        private static void RemoveDebugInfo(IScope scope, XElement result) {
            if (!scope.Get("debug_render", false)) {
                foreach (var element in result.DescendantsAndSelf()) {
                    element.SetAttributeValue("_file", null);
                    element.SetAttributeValue("_line", null);
                }
            }
        }

        public void SetContainerContext(IContainer container, IComponentDefinition component) {
            this._container = container;
        }

        public void OnContainerRelease() {
            
        }

        public void OnContainerCreateInstanceFinished() {
          
        }
    }
}