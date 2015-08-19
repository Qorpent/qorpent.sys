using System.Threading.Tasks;

namespace qorpent.v2.console {
    public interface IConsoleListener {
        Task<ConsoleCommandResult> Call(string command);
    }
}