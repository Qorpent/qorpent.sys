using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.tasks.factory {
    [ContainerComponent(Lifestyle.Singleton, ServiceType = typeof (ITaskFactory))]
    public class TaskFactory : ExtensibleServiceBase<ITaskSource>, ITaskFactory {
        public ITask Create(TaskEnvironment environment, XElement definition) {
            foreach (var taskSource in Extensions) {
                var task = taskSource.Create(environment, definition);
                if (null != task) {
                    return task;
                }
            }
            return null;
        }
    }
}