using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.tasks.tasks {
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ITask), Name = "qorpent.tasks.compound")]
    public class CompoundTask : TaskBase {
        public CompoundTask() {
            Main = new List<ITask>();
        }


        public IList<ITask> Main { get; set; }

        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);

            SetupTasks(environment, Main, config);
        }


        protected override void InternalExecute(IScope scope) {
            ExecuteSubTasks(Main, scope);
        }
    }
}