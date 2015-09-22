using System.Xml.Linq;
using Qorpent;

namespace qorpent.v2.tasks {
    public interface ITask {
        TaskFlags Flags { get; set; }
        void Initialize(TaskEnvironment environment, ITask parent, XElement config);
        void Execute(IScope scope);
    }
}