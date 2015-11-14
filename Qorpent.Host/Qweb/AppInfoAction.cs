using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qorpent.Security;
using Qorpent.IoC;
using Qorpent.Mvc;

namespace Qorpent.Host.Qweb
{
    [Action("info.app",Role=SecurityConst.ROLE_GUEST)]
    public class AppInfoAction:ActionBase
    {
        [Inject]
        public IHostConfigProvider ConfigProvider { get; set; }

        protected override object MainProcess() {
            var config = ConfigProvider.GetConfig();
            return config.Definition.Element("appinfo");
        }
    }
}
