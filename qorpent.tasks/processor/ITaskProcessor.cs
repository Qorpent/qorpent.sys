namespace qorpent.tasks.processor {
    public interface ITaskProcessor {
        void Execute(TaskEnvironment env);
    }
}