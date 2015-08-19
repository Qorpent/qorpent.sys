using System.IO;
using System.Threading.Tasks;
using Qorpent;

namespace qorpent.v2.console {
    public static class ConsoleContextExtensions {
        public static async Task<ConsoleCommandResult> ExecuteToFile(this IConsoleContext context, string filename, string command,
            string commandstring = null, IScope scope = null, bool append = false) {
            filename = EnvironmentInfo.ResolvePath(filename);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            if (!File.Exists(filename)) {
                append = false;
            }
            ConsoleCommandResult result = null;
            using (var s = new FileStream(filename, append ? FileMode.Append : FileMode.Create, FileAccess.Write)) {
                using (var sw = new StreamWriter(s)) {
                    var proxy = context.GetProxy(_ => {
                        _.Out = sw;
                        return _;
                    });
                    result =await proxy.Execute(command, commandstring, scope);
                    sw.Flush();
                }
            }
            return result;
        }
    }
}