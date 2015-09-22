using System;
using System.Xml.Linq;
using qorpent.tasks.processor;
using qorpent.v2.tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.factory {
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ITaskSource))]
    public class DefaultTaskSource : ServiceBase, ITaskSource {
        public ITask Create(TaskEnvironment environment, XElement definition) {
            if (definition.Name.LocalName == "task") {
                var name = definition.Attr("code");
                if (string.IsNullOrWhiteSpace(name)) {
                    throw new Exception("invalid generic task - no code");
                }
                var gt = ResolveService<ITask>(name);
                if (null == gt) {
                    throw new Exception("no configured task with name " + name);
                }
                return gt;
            }
            else {
                var name = "qorpent.tasks." + definition.Name.LocalName.ToLowerInvariant()+".task";
                var deft = ResolveService<ITask>(name);
                return deft;
            }
        }
    }
}