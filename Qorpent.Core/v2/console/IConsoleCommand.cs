using System.Threading.Tasks;
using Qorpent;

namespace qorpent.v2.console {
    public interface IConsoleCommand {
        Task<ConsoleCommandResult> Execute(IConsoleContext context, string commandname = null, string commandstring = null, IScope scope = null);
        int Priority { get; set; }
        bool IsMatch(string commandname, string commandstring, IScope scope);
    }
}