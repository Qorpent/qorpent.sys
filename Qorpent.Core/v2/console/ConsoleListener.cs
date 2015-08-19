using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Qorpent;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace qorpent.v2.console {
    public class ConsoleListener : IConsoleListener {
        private IConsoleContext _context;

        public ConsoleListener(IConsoleContext context,IScope scope = null) {
            _context = context;
            Scope = scope ?? new Scope();
        }

        IList<Task<ConsoleCommandResult>> PendingTasks = new List<Task<ConsoleCommandResult>>();
        public IScope Scope { get; set; }
        public int Run() {
            Log = Log ?? Loggy.Default;
            while (true) {
                try {
                    
                    _context.Write("> ");
                    var command = _context.ReadLine();
                    if (string.IsNullOrWhiteSpace(command)) {
                        continue;
                        
                    }
                    if (command == "sync") {
                        CheckTasks(true);
                        continue;
                    }
                    if (command == "exit" || command == "quit") {
                        try {
                            CheckTasks(true);
                        }
                        catch {

                        }
                        return 0;
                    }
                    CheckTasks();
                    var task = Call(command);
                    PendingTasks.Add(task);
                    Thread.Sleep(10);
                    CheckTasks();
                    Console.WriteLine();
                }
                catch (AggregateException ae) {
                    Log.Error(ae.InnerExceptions.FirstOrDefault());
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public Task<ConsoleCommandResult> Call(string command) {
            var rcommand =
                Regex.Match(command,
                    @"^\s*(?<name>[\S]+)\s*(?<args>[\s\S]*?)?\s*((?<redir>>>>?)\s+(?<file>[\s\S]+))?$");
            var name = rcommand.Groups["name"].Value;
            var args = rcommand.Groups["args"].Value;
            var redir = rcommand.Groups["redir"].Value;
            var file = rcommand.Groups["file"].Value;
            Task<ConsoleCommandResult> task = null;
            if (string.IsNullOrWhiteSpace(file)) {
                task = _context.Execute(name, args, Scope);
            }
            else {
                var append = redir == ">>>";
                task = _context.ExecuteToFile(file, name, args, Scope, append);
            }
            return task;
        }

        public ILoggy Log { get; set; }

        public void CheckTasks(bool wait = false,int timeout = 15000) {
            
            var tasks = PendingTasks.ToArray();
            if (wait) {
                Task.WaitAll(tasks, timeout);
            }
            foreach (var task in tasks) {
                if (!task.IsCompleted) {
                    continue;
                }
                try {
                    var result = task.Result;
                    if (result.Status != 0) {
                        var level = result.Status == -1 ? LogLevel.Error : LogLevel.Warn;
                        Log.Write(level, result.Status + ": " + result.StatusDescription, result.Error);
                    }

                }
                catch (AggregateException ae) {
                    Log.Error(ae.InnerExceptions.FirstOrDefault());
                }
                catch (Exception e) {
                    Log.Error(e);
                }
                PendingTasks.Remove(task);
            }
        }
    }
}