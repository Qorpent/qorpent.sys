using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.tasks.processor;
using qorpent.v2.tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.filesystem
{
    
    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(ITask),Name="qorpent.tasks.mkdir.task")]
    public class MkDirTask:TaskBase
    {
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            this.DirectoryToCreate = config.Attr("code");
        }

        public string DirectoryToCreate { get; set; }

        protected override void InternalExecute(IScope scope) {
            if (string.IsNullOrWhiteSpace(DirectoryToCreate)) {
                L.Warn(">>>>>  nothing to create");
            }
            var dir = DirectoryToCreate;
            dir = Interpolate(dir, scope);
            dir = EnvironmentInfo.ResolvePath(dir);
            L.Debug(">>>>>  real dir name: "+dir);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
                L.Info(">>>>> directory " + dir + " created ");
            }
            else {
                L.Trace("<====> directory "+dir+" exists");
            }
        }
    }
}
