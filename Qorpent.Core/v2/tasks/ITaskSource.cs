using System.Xml.Linq;
using qorpent.tasks.processor;

namespace qorpent.tasks.factory {
    public interface ITaskSource {
        ITask Create(TaskEnvironment environment, XElement definition);
    }
}