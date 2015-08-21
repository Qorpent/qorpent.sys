using System;
using System.IO;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;

namespace qorpent.v2.reports.renders
{
    public abstract class ReportRenderBase : IReportRender {

        public Uri Uri { get; set; }
        public string FileName { get; set; }
        public  virtual void Initialize(string uri, IScope scope) {
            Uri = new Uri(uri);
            var filepath = Uri.Host+"/"+ Uri.AbsolutePath;
            FileName = EnvironmentInfo.ResolvePath("@repos@/"+filepath);
            if (!File.Exists(FileName)) {
                throw new Exception("cannot find file "+FileName);
            }
        }

        public abstract IScope Render(IReportContext context, IScope scope, object item);
    }
}
