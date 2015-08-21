using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qorpent.v2.reports.agents;
using qorpent.v2.reports.core;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.reports.fake
{
    [ContainerComponent(Lifestyle.Singleton,Name="fakedata",ServiceType =typeof(IReportAgent))]
    public class FakeData:ReportAgentBase
    {
        public FakeData() {
            Phase = ReportPhase.Data;
        }
        public override async Task Execute(IReportContext context, ReportPhase phase, IScope scope = null) {
            context.Data["x"] = 1;
        }
    }

    [ContainerComponent(Lifestyle.Singleton, Name = "fakerender", ServiceType = typeof(IReportAgent))]
    public class FakeRender : ReportAgentBase
    {
        public FakeRender()
        {
            Phase = ReportPhase.Render;
        }
        public override async Task Execute(IReportContext context, ReportPhase phase, IScope scope = null)
        {
            context.SetHeader("Content-Type","text/html");
            context.Write("<textarea>");
            context.Write(context.Data.stringify());
            context.Write("</textarea>");
        }
    }
}
