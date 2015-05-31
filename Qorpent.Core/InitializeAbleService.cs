using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent {
    public class InitializeAbleService : ServiceBase {
        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        public virtual void Initialize() {
            if (null != ConfigProvider) {
                var e = ConfigProvider.GetConfig();
                if (null != e) {
                    InitializeFromXml(e);
                }
            }
        }

        public virtual void InitializeFromXml(XElement e) {
        }
    }
}