using qorpent.tasks.console;
using Qorpent.Utils;

namespace qorpent.tasks
{
    internal static class Program
    {
        private static int Main(string[] args) {
            return ConsoleApplication.Execute
                <TaskConsoleParameters>(
                args, 
                _ => new TaskProcess().Execute(_)
            );
        }
    }
}
