using System.Xml.Linq;

namespace qorpent.v2.tasks {
    public interface ITaskSource {
        ITask Create(TaskEnvironment environment, XElement definition);
    }
}