using System.Xml.Linq;
using qorpent.v2.model;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.core {
    public class ListItem : Item {
        public string IfCondition { get; set; }
        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            IfCondition = xml.AttrOrValue("ng-if");
        }

        protected override void LoadFromJson(object jsonsrc) {
            base.LoadFromJson(jsonsrc);
            var j = jsonsrc.nestorself("_source");
            IfCondition = j.str("ngif");
        }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("ngif",IfCondition);

        }
    }
}