using System;
using System.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.console {
    public class TaskProcess {
        public int Execute(TaskConsoleParameters args) {
            var context = BSharpCompiler.CompileDirectory(args.ScriptDirectory,
                new BSharpConfig {Global = new Scope(args.Global),KeepLexInfo = true});
            var cls = context[args.ScriptClass];
            if (null == cls) {
                throw new Exception("cannot find class " + args.ScriptClass);
            }
            var container = ContainerFactory.CreateDefault();
            container.RegisterAssembly(typeof (TaskProcess).Assembly);

            var configProvider = new GenericConfiguration(cls.Compiled, context) {Custom = args};
            container.Set<IConfigProvider>(configProvider);
            Loggy.Manager = container.Get<ILoggyManager>();
            var defloggy = Loggy.Manager.Get();
            defloggy.Level = args.LogLevel;
            var consoleAppender = defloggy.Appenders.OfType<ConsoleAppender>().FirstOrDefault();
            if (null == consoleAppender) {
                defloggy.Appenders.Add(new ConsoleAppender {
                    Format = args.LogFormat,
                    Level = args.LogLevel,
                    Manager = Loggy.Manager
                });
            }
            else {
                consoleAppender.Format = args.LogFormat;
                consoleAppender.Level = args.LogLevel;
            }
            var loggy = Loggy.Manager.Get("bcinst");
            var installRequest = new TaskEnvironment {
                Config = cls.Compiled,
                Context = context,
                Log = loggy,
                Globals = new Scope(args.Global),
                Targets = args.Targets.ToArray()
            };
            var processor = container.Get<ITaskProcessor>();
            processor.Execute(installRequest);
            return 0;
        }
    }
}