using System;
using System.IO;
using System.Threading.Tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils;

namespace qorpent.v2.console {
    public interface IConsoleContext {
        TextWriter Out { get; set; }
        TextWriter Error { get; }
        string ReadLine(string message = null);
        ConsoleKeyInfo ReadKey();
        IConsoleContext SetColor(ConsoleColor color);
        IConsoleContext ResetColor();
        IConsoleContext Write(string data);
        IConsoleContext WriteLine(string data);
        ConsoleApplicationParameters Parameters { get; }
        IContainer Container { get; }
        ConsoleCallInfo Info { get; }
        Task<ConsoleCommandResult> Execute(string commandname, string commandstring, IScope scope);
        IConsoleCommand GetCommand(string commandname, string commandstring = null, IScope scope = null);
        IConsoleContext GetProxy(Func<IConsoleContext,IConsoleContext> setup = null);
    }
}