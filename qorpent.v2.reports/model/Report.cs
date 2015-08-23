using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.v2.model;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

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
        public string OnForm { get; set; }
        public string OnQuery { get; set; }

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
            OnForm = jsonsrc.str("onform");
            OnQuery = jsonsrc.str("onquery");
        }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("onform",OnForm,true);
            jw.WriteProperty("onquery",OnQuery,true);
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
            OnForm = xml.Attr("onform");
            OnQuery = xml.Attr("onquery");
            var xagents = xml.Elements("agent");
            foreach (var element in xagents) {
                Agents.Add(Create<ReportAgentDefintion>(element));   
            }
        }

        public object Definition { get; set; }
    }
}
