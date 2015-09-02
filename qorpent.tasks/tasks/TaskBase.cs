using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.tasks.factory;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace qorpent.tasks.tasks {
    public abstract class TaskBase : ITask {
        public TaskBase() {
            Name = GetType().Name;
            Before = new List<ITask>();
            After = new List<ITask>();
            Fallback = new List<ITask>();
            this.Interpolator = new StringInterpolation {AncorSymbol = '@'};
        }

        public StringInterpolation Interpolator { get; set; }

        public ITaskFactory TaskFactory { get; set; }

        public string Name { get; set; }

        public LogLevel ErrorLevel { get; set; }
        public string Condition { get; set; }

        public IList<ITask> Before { get; set; }
        public IList<ITask> After { get; set; }
        public IList<ITask> Fallback { get; set; }
        public ILoggy L { get; set; }

        public XElement Config { get; set; }

        public ITask Parent { get; set; }

        public TaskEnvironment Environment { get; set; }

        public virtual void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            Environment = environment;
            TaskFactory = TaskFactory ?? environment.Container.Get<ITaskFactory>();
            Parent = parent;
            Config = config;
            L = Environment.Log;
            ErrorLevel = config.Attr("errorlevel", "Error").To<LogLevel>();
            Condition = config.Attr("condition");
            var name = config.Attr("name");
            if (!string.IsNullOrWhiteSpace(name)) {
                Name = name;
            }
            if (config.Attr("async").ToBool()) {
                Flags |= TaskFlags.Async;
            }
            SetupTasks(environment, Before, config.Element("before"));
            SetupTasks(environment, After, config.Element("after"));
            SetupTasks(environment, Fallback, config.Element("fallback"));

            TaskCode = config.Attr("code");
            TaskName = config.Attr("name");
        }

        public void Execute(IScope scope) {
            if (IsMatch(scope)) {
                try {
                    L.Debug(">>> task " + Name);
                    ExecuteSubTasks(Before, scope);
                    InternalExecute(scope);
                    ExecuteSubTasks(After, scope);
                    L.Debug("<<< task " + Name);
                }
                catch (Exception e) {
                    L.Write(ErrorLevel, "<<< error in " + Name + " : " + e);
                    try {
                        ExecuteSubTasks(Fallback, scope);
                    }
                    catch (Exception fe) {
                        L.Error("ERROR IN FALLBACK OF " + Name + " : " + fe);
                    }
                    if (ErrorLevel >= LogLevel.Error) {
                        throw e;
                    }
                }
            }
            else {
                L.Debug("<==> " + Name + " ignored ");
            }
        }

        public TaskFlags Flags { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }

        protected void ExecuteSubTasks(IEnumerable<ITask> tasks, IScope scope) {
            var ts = tasks.ToArray();
            var asyncs = ts.Where(_ => _.Flags.HasFlag(TaskFlags.Async)).ToArray();
            var syncs = ts.Where(_ => !_.Flags.HasFlag(TaskFlags.Async)).ToArray();
            IList<Task> waiters = new List<Task>();
            foreach (var w in asyncs) {
                var s = new Scope(scope);
                waiters.Add(Task.Run(() => w.Execute(s)));
            }
            foreach (var sync in syncs) {
                var s = new Scope(scope);
                sync.Execute(s);
            }
            if (0 != waiters.Count) {
                Task.WaitAll(waiters.ToArray());
            }
        }

        protected virtual bool IsMatch(IScope scope) {
            if (string.IsNullOrWhiteSpace(Condition)) {
                return true;
            }
            return new LogicalExpressionEvaluator().Eval(Condition, scope);
        }

        protected abstract void InternalExecute(IScope scope);


        protected void SetupTasks(TaskEnvironment environment, IList<ITask> target, XElement config) {
            if (null == config) {
                return;
            }
            foreach (var element in config.Elements()) {
                var name = element.Name.LocalName;
                if (name == "before" || name == "after" || name == "fallback" || name == "project") {
                    continue;
                }
                if (null == TaskFactory) {
                    throw new Exception("no task factory configured");
                }
                var task = TaskFactory.Create(environment, element);
                if (null != task) {
                    target.Add(task);
                    task.Initialize(environment, this, element);
                }
            }
        }

        protected string Interpolate(string str, IScope scope) {
            var s = GetInterpolationScope(scope);
            return Interpolator.Interpolate(str, s);
        }

        protected virtual IScope GetInterpolationScope(IScope scope) {
            return scope;
        }
    }
}