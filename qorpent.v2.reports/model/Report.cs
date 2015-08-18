using System.Xml.Linq;
using qorpent.v2.model;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.model
{
    [ContainerComponent(Lifestyle.Transient,Name="qorpent.reports.model.report",ServiceType = typeof(IReport))]
    public class Report : Item,IReport
    {
        public string Role { get; set; }
        public string RequestProcessor { get; set; }
        public string HtmlGenerator { get; set; }
        public string DataGenerator { get; set; }

        public override void Merge(IItem src) {
            base.Merge(src);
            var report = src as Report;
            if (null != report) {
                if (!string.IsNullOrWhiteSpace(report.RequestProcessor)) {
                    RequestProcessor = report.RequestProcessor;
                }
                if (!string.IsNullOrWhiteSpace(report.HtmlGenerator))
                {
                    HtmlGenerator = report.HtmlGenerator;
                }
                if (!string.IsNullOrWhiteSpace(report.DataGenerator))
                {
                    DataGenerator = report.DataGenerator;
                }
            }
        }

        protected override void LoadFromJson(object jsonsrc) {
            base.LoadFromJson(jsonsrc);
            var src = jsonsrc.nestorself("_source");
            RequestProcessor = src.str("requestprocessor");
            HtmlGenerator = src.str("htmlgenerator");
            DataGenerator = src.str("datagenerator");
        }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("requestprocessor",RequestProcessor);
            jw.WriteProperty("htmlgenerator", HtmlGenerator);
            jw.WriteProperty("datagenerator", DataGenerator);
        }

        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            RequestProcessor = xml.AttrOrElement("requestprocessor");
            HtmlGenerator = xml.AttrOrElement("htmlgenerator");
            DataGenerator = xml.AttrOrElement("htmlgenerator");

        }
    }
}
