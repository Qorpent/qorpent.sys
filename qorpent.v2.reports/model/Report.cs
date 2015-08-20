using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.v2.model;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.reports.model
{
    [ContainerComponent(Lifestyle.Transient,Name="qorpent.reports.model.report",ServiceType = typeof(IReport))]
    public class Report : Item,IReport
    {
        public Report() {
            Agents = new List<IReportAgentDefinition>();
        }
        public string Role { get; set; }
        public IList<IReportAgentDefinition> Agents { get; set; }

        public override void Merge(IItem src) {
            base.Merge(src);
            var report = src as Report;
            if (null != report) {
              
            }
        }

        protected override void LoadFromJson(object jsonsrc) {
            base.LoadFromJson(jsonsrc);
            var src = jsonsrc.nestorself("_source");
            var ags = src.arr("agents");
            if (null != ags && 0 != ags.Length) {
                foreach (var ag in ags) {
                    Agents.Add(Create<ReportAgentDefintion>(ag));
                }
            }
        }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.OpenProperty("agents");
            jw.OpenArray();
            foreach (var agentDefinition in Agents) {
               jw.WriteObject(agentDefinition,mode);
            }
            jw.CloseArray();
            jw.CloseProperty();
        }

        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            var xagents = xml.Elements("agent");
            foreach (var element in xagents) {
                Agents.Add(Create<ReportAgentDefintion>(element));   
            }
        }

        public object Definition { get; set; }
    }
}
