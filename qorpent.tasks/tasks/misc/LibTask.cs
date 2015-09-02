using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.tasks.factory;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.misc
{
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ITask), Name = "qorpent.tasks.lib.task")]
    public class LibTask:TaskBase
    {
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            var libname = config.Attr("code");
            if (environment.LibNameCache.Contains(libname)) {
                L.Trace("lib " + libname + " already in cache");
            }
            else {
                environment.Container.RegisterAssembly(Assembly.Load(libname));
                var factory = environment.Container.Get<ITaskFactory>();
                factory.UpdateSourceList();
                L.Trace("lib "+libname+" registered in Container");
            }
        }

        protected override void InternalExecute(IScope scope) {
            
        }
    }
}
