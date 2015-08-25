using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using qorpent.v2.model;
using qorpent.v2.reports.core;
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
            Parameters = new Dictionary<string, ReportParameter>();
        }
        public string Role { get; set; }
        public IList<IReportAgentDefinition> Agents { get; set; }
        public string OnForm { get; set; }
        public string OnQuery { get; set; }
        public string ToolView { get; set; }

        public override void Merge(IItem src) {
            base.Merge(src);
            var report = src as Report;
            if (null != report) {
                foreach (var p  in report.Parameters) {
                    Parameters[p.Key] = p.Value;
                }
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
            ToolView = jsonsrc.str("toolview");
            var parameters = jsonsrc.arr("parameters");
            if (null != parameters && 0 != parameters.Length) {
                SetupParameters(parameters.Select(Create<ReportParameter>));
            }
            var dparameters = jsonsrc.map("parameters");
            if (null != dparameters) {
                SetupParameters(dparameters.Select(_ => {
                    var i = new ReportParameter();
                    var s = _.Value as string;
                    if (s != null) {
                        i.Name = s;
                    }
                    else {
                        i.Read(_.Value);
                    }
                    if (string.IsNullOrWhiteSpace(i.Id)) {
                        i.Id = _.Key;
                    }
                    if (i.Id == "_empty_") {
                        i.Id = "";
                    }
                    return i;
                }));
            }

        }

        private void SetupParameters(IEnumerable<ReportParameter> rawparameters) {
            var idx = 1000;
            var ps = rawparameters.ToArray();
            foreach (var p in ps  ) {
                if (p.Idx == 0) {
                    p.Idx = idx++;
                }
                if (string.IsNullOrWhiteSpace(p.Default)) {
                    if (p.List.Count > 0) {
                        p.Default = p.List[0].Id;
                    }
                    else {
                        p.Default = "";
                    }
                }
                else if (p.Default == "_empty_") {
                    p.Default = "";
                }
            }
            foreach (var p in ps.OrderBy(_=>_.Idx)) {
                Parameters[p.Id] = p;
            }
        }

    

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            mode = mode ?? "";
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("onform",OnForm,true);
            jw.WriteProperty("onquery",OnQuery,true);
            jw.WriteProperty("toolview",ToolView,true);
            if (!mode.Contains("noagents")) {
                jw.OpenProperty("agents");
                jw.OpenArray();
                foreach (var agentDefinition in Agents) {
                    jw.WriteObject(agentDefinition, mode);
                }

                jw.CloseArray();
                jw.CloseProperty();
                
            }
            if (!mode.Contains("noparams"))
            {
                jw.WriteProperty("parameters", Parameters.Select(_=>_.Value as object).ToArray());
            }
        }

        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            OnForm = xml.Attr("onform");
            OnQuery = xml.Attr("onquery");
            ToolView = xml.Attr("toolview");
            var xagents = xml.Elements("agent");
            foreach (var element in xagents) {
                Agents.Add(Create<ReportAgentDefintion>(element));   
            }
            var parameters = xml.Elements("param");
            SetupParameters(parameters.Select(Create<ReportParameter>));
        }

        public object Definition { get; set; }
        public IDictionary<string,ReportParameter> Parameters { get; set; }
    }
}
