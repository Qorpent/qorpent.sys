using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;

namespace qorpent.tasks {
    public interface ITask {
        TaskFlags Flags { get; set; }
        void Initialize(TaskEnvironment environment, ITask parent, XElement config);
        void Execute(IScope scope);
    }
}