using System.Linq;
using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log.NewLog;

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

        public void UpdateSourceList() {
            var sources = Container.All<ITaskSource>();
            foreach (var taskSource in sources) {
                if (Extensions.All(_ => _.GetType() != taskSource.GetType())) {
                    Logg.Trace("taskfactory add source of type: "+taskSource.GetType().FullName);
                    Extensions.Add(taskSource);
                }
            }
        }
    }
}