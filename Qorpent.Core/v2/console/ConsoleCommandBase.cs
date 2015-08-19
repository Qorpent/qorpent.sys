using System;
using System.Threading.Tasks;
using Qorpent;

namespace qorpent.v2.console {
    public abstract class ConsoleCommandBase : ServiceBase, IConsoleCommand {
        public async Task<ConsoleCommandResult> Execute(IConsoleContext context, string commandname = null, string commandstring = null, IScope scope = null) {
            var result = new ConsoleCommandResult();
            try {
                await InternalExecute(context,result, commandname, commandstring, scope);
            }
            catch (Exception e) {
                if (0 == result.Status) {
                    result.Status = -1;
                }
                if (string.IsNullOrWhiteSpace(result.StatusDescription)) {
                    result.StatusDescription = e.GetType().Name + ": " + e.Message;
                }
                if (null == result.Error) {
                    result.Error = e;
                }
            }
            return result;
        }

        protected abstract Task InternalExecute(IConsoleContext context, ConsoleCommandResult result, string commandname, string commandstring, IScope scope);

        public int Priority { get; set; }
        public virtual bool IsMatch(string commandname, string commandstring, IScope scope) {
            var name = this.GetType().Name.ToLowerInvariant().Replace("command", "");
            if (commandname == name) return true;
            return false;
        }
    }
}