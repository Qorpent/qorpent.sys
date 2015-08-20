using System.Xml.Linq;
using qorpent.v2.model;
using qorpent.v2.reports.core;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.model {
    [ContainerComponent(Lifestyle.Transient, Name = "qorpent.reports.model.agentdefinition", ServiceType = typeof(IReportAgentDefinition))]
    public class ReportAgentDefintion:Item, IReportAgentDefinition
    {
        public ReportPhase Phase { get; set; }
        public object Definition { get; set; }
        public int Idx { get; set; }
        public bool Parallel { get; set; }

        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            Phase = xml.Attr("phase").To<ReportPhase>(true);
            Parallel = xml.Attr("parallel").ToBool();
        }

        protected override void LoadFromJson(object jsonsrc) {
            base.LoadFromJson(jsonsrc);
            var src = jsonsrc.nestorself("_source");
            Phase = src.str("phase").To<ReportPhase>(true);
            Parallel = src.bul("parallel");
        }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("phase",Phase);
            jw.WriteProperty("parallel",Parallel);
        }
    }
}