using qorpent.v2.tasks;

namespace qorpent.tasks.factory {
    public interface ITaskFactory : ITaskSource {
        void UpdateSourceList();
    }
}