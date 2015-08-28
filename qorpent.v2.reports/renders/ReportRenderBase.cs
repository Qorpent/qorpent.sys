using System;
using System.IO;
using System.Xml.Linq;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.renders
{
    public abstract class ReportRenderBase : IReportRender {

        public ReportRenderBase() {
            ResolveToFile = true;
        }
        public Uri Uri { get; set; }
        public bool ResolveToFile { get; set; }
        public string FileName { get; set; }
        public  virtual void Initialize(string uri, IScope scope) {
            Uri = new Uri(uri);
            ResolveFileName();
        }

        private void ResolveFileName() {
            if (ResolveToFile) {
                var filepath = Uri.Host + "/" + Uri.AbsolutePath;

                FileName = EnvironmentInfo.ResolvePath("@repos@/" + filepath);
                var f = FileName;
                if (!File.Exists(f)) {
                    f = FileName + ".xml";
                    if (!File.Exists(f)) {
                        f = FileName + ".bxl";
                    }
                    if (!File.Exists(f)) {
                        throw new Exception("cannot find file " + FileName);
                    }
                    FileName = f;
                }
            }
        }

        public abstract IScope Render(IReportContext context, IScope scope, object item);

        protected virtual void Finalize(IReportContext context, IScope scope, object result) {
            if (scope.Get("store_render").ToBool()) {
                scope[scope.Get("render_name", "render_result")] = result;
            }
            if (!scope.Get("no_render").ToBool()) {
                context.Write(result.ToString());
            }
        }
    }
}
